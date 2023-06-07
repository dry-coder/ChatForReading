﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Web.Data;
using Chat.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Chat.Web.Hubs;
using Chat.Web.ViewModels;
using System.Text.RegularExpressions;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3;
using Azure.Core;
using NuGet.Common;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Embeddings;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using System.IO;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenAI;
using OpenAI.Completions;
using OpenAI.Embeddings;
using OpenAI.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound();

            var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
            return Ok(messageViewModel);
        }

        [HttpGet("Room/{roomName}")]
        public IActionResult GetMessages(string roomName)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return BadRequest();

            var messages = _context.Messages.Where(m => m.ToRoomId == room.Id)
                .Include(m => m.FromUser)
                .Include(m => m.ToRoom)
                .OrderByDescending(m => m.Timestamp)
                .Take(20)
                .AsEnumerable()
                .Reverse()
                .ToList();

            var messagesViewModel = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messagesViewModel);
        }

        [HttpGet("File/{roomName}")]
        public IActionResult GetFiles(string roomName)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return BadRequest();

            var messages = _context.Messages.Where(m => ((m.ToRoomId == room.Id) && (m.Content.Contains("Files of"))))
                .Include(m => m.FromUser)
                .Include(m => m.ToRoom)
                .OrderByDescending(m => m.Timestamp)
                .Take(20)
                .AsEnumerable()
                .Reverse()
                .ToList();

            var messagesViewModel = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messagesViewModel);
        }

        private const string OpenaiApiKeyFileName = "OpenAI-API.key";
        //const string OPENAPI_TOKEN = "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";//输入自己的api-key
        private static OpenAI.OpenAIClient? Api;
        private const double HighSimilarityThreshold = 0.8;
        private const string EmbeddingsFolder = "wwwroot/uploads/Embeddings/";

        private const string SumFolder = "wwwroot/uploads/SumFolder/";
        private const string RecordFolder = "wwwroot/uploads/RecordFolder/";

        private string NowSaveFile = "";

        [HttpPost]
        public async Task<ActionResult<Message>> Create(MessageViewModel viewModel)
        {
            string OPENAPI_TOKEN = "";
            if (System.IO.File.Exists(OpenaiApiKeyFileName))
                OPENAPI_TOKEN = System.IO.File.ReadAllText(OpenaiApiKeyFileName);
            var ss = "";
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var room = _context.Rooms.FirstOrDefault(r => r.Name == viewModel.Room);
            if (room == null)
                return BadRequest();

            var msg = new Message()
            {
                Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
                FromUser = user,
                ToRoom = room,
                Timestamp = DateTime.Now
            };

            NowSaveFile = msg.ToRoom.Id.ToString();

            //------------------------search-----------------------
            Api = new OpenAIClient(OPENAPI_TOKEN, OpenAI.Models.Model.Ada);
            string RegexCallSearch = @"@search\s*";
            MatchCollection SearchCall = Regex.Matches(msg.Content, RegexCallSearch);
            if (SearchCall.Count > 0)
            {
                string requestStr = msg.Content;
                requestStr = requestStr.Remove(0, 7);

                EmbeddingsResponse? queryEmbed = await Api.EmbeddingsEndpoint.CreateEmbeddingAsync(requestStr);
                Debug.Assert(queryEmbed != null);

                double[] queryVector = queryEmbed.Data.SelectMany(datum => datum.Embedding).ToArray();
                queryVector = NormalizeVector(queryVector); // Normalize the query vector

                // Get all json files from the folder
                var jsonFiles = Directory.GetFiles(EmbeddingsFolder + NowSaveFile, "*.json");
                
                var documentVectors = new List<double[]>();
                var documents = new List<string>();
                foreach (var filePath in jsonFiles)
                {
                    string embeddingsJson = await System.IO.File.ReadAllTextAsync(filePath);
                    var fileData = JsonSerializer.Deserialize<ConsolidatedEmbeddingsFileData>(embeddingsJson);
                    Debug.Assert(fileData != null);

                    foreach (var embeddingObject in fileData.embeddings)
                    {
                        double[] documentVector = embeddingObject.embeddings.Data.SelectMany(datum => datum.Embedding).ToArray();
                        documentVector = NormalizeVector(documentVector); // Normalize the document vector
                        documentVectors.Add(documentVector);
                        documents.Add(embeddingObject.text);
                    }
                }

                var similarities = new List<double>();
                for (int i = 0; i < documentVectors.Count; i++)
                {
                    similarities.Add(CosineSimilarity(queryVector, documentVectors[i]));
                }

                var sortedDocuments = documents.Zip(similarities, (d, s) => new { Document = d, Similarity = s })
                    .OrderByDescending(x => x.Similarity)
                    .ToList();

                // Build the context from all highly similar documents
                const int maxChars = 4096;
                var contextBuilder = new StringBuilder();
                var tokenCount = 0;

                foreach (var doc in sortedDocuments.Where(x => x.Similarity >= HighSimilarityThreshold))
                {
                    var docTokens = System.Text.Encoding.UTF8.GetByteCount(doc.Document);
                    if (tokenCount + docTokens > maxChars)
                        break; // Stop if the next document would exceed the maximum token count

                    contextBuilder.AppendLine(doc.Document).AppendLine();
                    tokenCount += docTokens;
                }
                var context = contextBuilder.ToString();
                
                if (tokenCount == 0 || string.IsNullOrWhiteSpace(context))
                { 
                    context += "[[No good matches found.]]\n";
                }
                string completeQuery = @$"The following information is provided for context: \n\n{context} \n\n Given this information, can you please answer the following question: \n\n ""{requestStr}""?";
                CompletionResult? result = await Api.CompletionsEndpoint.CreateCompletionAsync(completeQuery, model: OpenAI.Models.Model.Davinci, temperature: 0.7, max_tokens: 256);

                msg.Content += " [[Judge]]:"+result.ToString().TrimStart();
                /*msg.Content += " [[result]]:\n";
                msg.Content += context;*/

            }
            //-----------------------------------------------------

            //-------------------------gpt--------------------------
            string RegexCallGpt = @"@gpt\s*";
            string RegexCallSumBegin = @"@sum_begin\s*";
            string RegexCallSumEnd = @"@sum_end\s*";

            MatchCollection GptCall = Regex.Matches(msg.Content,RegexCallGpt);
            MatchCollection GptSumBegin = Regex.Matches(msg.Content, RegexCallSumBegin);
            MatchCollection GptSumEnd = Regex.Matches(msg.Content, RegexCallSumEnd);

            if (GptCall.Count > 0)
            {
                string requestStr;
                requestStr = msg.Content + "(请将回答限制在200字内)";
                OpenAIService service = new OpenAIService(new OpenAiOptions() { ApiKey = OPENAPI_TOKEN });
                CompletionCreateRequest createRequest = new CompletionCreateRequest()
                {

                    Prompt = requestStr,
                    Temperature = 0.3f,
                    MaxTokens = 1000
                };

                var res = await service.Completions.CreateCompletion(createRequest, OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);
               
                if (res.Successful)
                {
                    ss = res.Choices.FirstOrDefault().Text;
                    msg.Content += "[[answer]]:";
                    msg.Content += ss;
                }
            }

            if (GptSumBegin.Count > 0)
            {
                int specific = msg.ToRoom.Id;
                string destination = RecordFolder + specific + ".txt";
                string content = msg.FromUser.FullName + ": 会议开始！！" + "\n";
                System.IO.File.WriteAllText(@destination, content);
            }
            else
            {
                int specific = msg.ToRoom.Id;
                string destination = RecordFolder + specific + ".txt";
                string content = msg.FromUser.FullName + "::" + msg.Content+"\n";
                System.IO.File.AppendAllText(@destination, content);
            }

            if (GptSumEnd.Count > 0)
            {
                int specific = msg.ToRoom.Id;
                string destinationRecord = RecordFolder + specific + ".txt";
                string content = msg.FromUser.FullName + ":" + msg.Content;

                //System.IO.File.AppendAllText(@destinationRecord, content);

                string filename = DateTime.Now.ToString("yyyymmddMMss") + "_" + specific + ".txt";
                string destinationSum = SumFolder + NowSaveFile + "/" +  filename;
                if (!Directory.Exists(SumFolder + NowSaveFile))
                    Directory.CreateDirectory(SumFolder + NowSaveFile);

                string requestStr;
                requestStr = System.IO.File.ReadAllText(@destinationRecord) + "请总结上面的对话，形成一份会议记录";
                OpenAIService service = new OpenAIService(new OpenAiOptions() { ApiKey = OPENAPI_TOKEN });
                CompletionCreateRequest createRequest = new CompletionCreateRequest()
                {

                    Prompt = requestStr,
                    Temperature = 0.3f,
                    MaxTokens = 1000
                };

                var res = await service.Completions.CreateCompletion(createRequest, OpenAI.GPT3.ObjectModels.Models.TextDavinciV3);

                if (res.Successful)
                {
                    ss = res.Choices.FirstOrDefault().Text;
                    System.IO.File.WriteAllText(@destinationSum, ss);

                    string htmlImage = string.Format(
                    "<h1>【Files of Sum】" + filename.ToString() + "</h1><a href=\"/uploads/SumFolder/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/SumFolder/{0}\" class=\"post-image\">" +
                    "</a>", NowSaveFile + "/" + filename);

                    msg = new Message()
                    {
                        Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                        Timestamp = DateTime.Now,
                        FromUser = user,
                        ToRoom = room
                    };
                }
                else
                {
                    string htmlImage = string.Format(
                    "Sum Failed!!");

                    msg = new Message()
                    {
                        Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                        Timestamp = DateTime.Now,
                        FromUser = user,
                        ToRoom = room
                    };
                }
            }

            //--------------------------------------------------------

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Broadcast the message
            var createdMessage = _mapper.Map<Message, MessageViewModel>(msg);
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages
                .Include(u => u.FromUser)
                .Where(m => m.Id == id && m.FromUser.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (message == null)
                return NotFound();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);

            return Ok();
        }



        private static double[] NormalizeVector(double[] vector)
        {
            double length = Math.Sqrt(vector.Sum(x => x * x));
            return vector.Select(x => x / length).ToArray();
        }

        private static double CosineSimilarity(double[] vector1, double[] vector2)
        {
            double dotProduct = 0.0f;
            double magnitude1 = 0.0f;
            double magnitude2 = 0.0f;
            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = (float)Math.Sqrt(magnitude1);
            magnitude2 = (float)Math.Sqrt(magnitude2);
            if (magnitude1 != 0.0f && magnitude2 != 0.0f)
            {
                return dotProduct / (magnitude1 * magnitude2);
            }
            else
            {
                return 0.0f;
            }
        }

#pragma warning disable 8618
        public class EmbeddingData
        {
            public EmbeddingsResponse embeddings { get; set; }
            public string text { get; set; }
        }

        public class ConsolidatedEmbeddingsFileData
        {
            public List<EmbeddingData> embeddings { get; set; }
            public string sourceFileName { get; set; }
        }
#pragma warning restore 8618
    }
}
