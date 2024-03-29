﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using OpenAI;
using OpenAI.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Chat.Web.Data;
using Chat.Web.Helpers;
using Chat.Web.Hubs;
using Chat.Web.Models;
using Chat.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowedExtensions;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileValidator _fileValidator;

        private const string OpenaiApiKeyFileName = "OpenAI-API.key";
        //private const string apiKey = "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";
        private static OpenAIClient? Api;
        private const string InputsFolder = "wwwroot/uploads/Files/";
        private const string EmbeddingsFolder = "wwwroot/uploads/Embeddings/";

        public string NowSaveFile;

        public UploadController(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment environment,
            IHubContext<ChatHub> hubContext,
            IConfiguration configruation,
            IFileValidator fileValidator)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
            _hubContext = hubContext;
            _fileValidator = fileValidator;

            FileSizeLimit = configruation.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowedExtensions = configruation.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([FromForm] UploadViewModel viewModel)
        {
            string apiKey = "";
            if (System.IO.File.Exists(OpenaiApiKeyFileName))
                apiKey = System.IO.File.ReadAllText(OpenaiApiKeyFileName);

            NowSaveFile = viewModel.RoomId.ToString();

            if (ModelState.IsValid)
            {
               if (!_fileValidator.IsValid(viewModel.File))
                    return BadRequest("Validation failed!!!");

                //var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModel.File.FileName);
                var fileName = Path.GetFileName(viewModel.File.FileName);

                var folderPath = Path.Combine(_environment.WebRootPath, "uploads/Files/"+NowSaveFile);
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.File.CopyToAsync(fileStream);
                }

                var user = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var room = _context.Rooms.Where(r => r.Id == viewModel.RoomId).FirstOrDefault();
                if (user == null || room == null)
                    return NotFound();

                var extension = Path.GetExtension(viewModel.File.FileName).ToLowerInvariant();

                string htmlImage = string.Format(
                    "<a href=\"/uploads/Files/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/Files/{0}\" class=\"post-image\">" +
                    "</a>", NowSaveFile + "/" + fileName);

                if (extension == ".pdf")
                {
                    htmlImage = string.Format(
                    "<h1>【Files of PDF】" + fileName.ToString() + "</h1><a href=\"/uploads/Files/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/Files/{0}\" class=\"post-image\">" +
                    "</a>", NowSaveFile + "/" + fileName);

                    string outFileName = filePath.Replace(".pdf", ".txt");
                    if (!System.IO.File.Exists(outFileName))
                        System.IO.File.Delete(outFileName);
                    using (PdfDocument document = PdfDocument.Open(filePath))
                    {
                        StringBuilder extractedText = new StringBuilder();

                        foreach (Page page in document.GetPages())
                        {
                            var words = page.GetWords(NearestNeighbourWordExtractor.Instance);
                            var blocks = RecursiveXYCut.Instance.GetBlocks(words);

                            foreach (var block in blocks)
                            {
                                extractedText.AppendLine(block.Text);
                            }
                        }
                        System.IO.File.WriteAllText(outFileName, extractedText.ToString(), Encoding.UTF8);
                    }

                    Api = new OpenAIClient(apiKey, Model.Ada);

                    // Create directory to store embeddings
                    Directory.CreateDirectory(EmbeddingsFolder + NowSaveFile);

                    // Get all txt files from the folder
                    var txtFilesTemp = Directory.GetFiles(InputsFolder + NowSaveFile, fileName.Replace(".pdf", ".txt"));

                    // Process each file
                    foreach (var filePathTemp in txtFilesTemp)
                    {
                        await ProcessFile(filePathTemp, EmbeddingsFolder + NowSaveFile);
                    }

                }
                if (extension == ".txt")
                {
                    htmlImage = string.Format(
                    "<h1>【Files of TXT】" + fileName.ToString() + "</h1><a href=\"/uploads/Files/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/Files/{0}\" class=\"post-image\">" +
                    "</a>", NowSaveFile + "/" + fileName);

                    Api = new OpenAIClient(apiKey, Model.Ada);

                    // Create directory to store embeddings
                    Directory.CreateDirectory(EmbeddingsFolder + NowSaveFile);

                    // Get all txt files from the folder
                    var txtFilesTemp = Directory.GetFiles(InputsFolder + NowSaveFile, fileName);

                    // Process each file
                    foreach (var filePathTemp in txtFilesTemp)
                    {
                        await ProcessFile(filePathTemp, EmbeddingsFolder + NowSaveFile);
                    }
                }

                /*
                Api = new OpenAIClient(apiKey, Model.Ada);

                // Create directory to store embeddings
                Directory.CreateDirectory(EmbeddingsFolder);

                // Get all txt files from the folder
                var txtFiles = Directory.GetFiles(InputsFolder + NowSaveFile, "*.txt");

                // Process each file
                foreach (var filePathTemp in txtFiles)
                {
                    await ProcessFile(filePathTemp);
                }*/


                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    Timestamp = DateTime.Now,
                    FromUser = user,
                    ToRoom = room
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // Send image-message to group
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        private static async Task ProcessFile(string filePath, string embedlocation)
        {
            string input = await System.IO.File.ReadAllTextAsync(filePath);
            string filename = Path.GetFileName(filePath);

            // Split the input into sections
            const int maxSectionLength = 1024;
            var sections = SplitIntoSections(input, maxSectionLength);

            List<object> embeddingsList = new List<object>();

            int partCount = 0;
            foreach (var section in sections)
            {
                var result = await Api.EmbeddingsEndpoint.CreateEmbeddingAsync(section);

                // Create an object that includes the embeddings, the original text
                var embeddingObject = new
                {
                    embeddings = result,
                    text = section
                };

                embeddingsList.Add(embeddingObject);
                partCount++;
            }

            await WriteAllEmbeddingsToFile(filename, embeddingsList, embedlocation);
        }

        private static List<string> SplitIntoSections(string text, int maxSectionLength)
        {
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sections = new List<string>();
            var currentSection = new StringBuilder();

            foreach (var line in lines)
            {
                if (currentSection.Length + line.Length > maxSectionLength && currentSection.Length > 0)
                {
                    // if adding this line would exceed the maxSectionLength, start a new section
                    sections.Add(currentSection.ToString());
                    currentSection.Clear();
                }

                // simple heuristic: a line followed by an empty line is a header
                if (line.Trim().Length == 0 && currentSection.Length > 0) // we've hit a header, start a new section
                {
                    sections.Add(currentSection.ToString());
                    currentSection.Clear();
                }
                else
                {
                    currentSection.AppendLine(line);
                }
            }

            // Add the final section if it's not empty
            if (currentSection.Length > 0)
            {
                sections.Add(currentSection.ToString());
            }

            return sections;
        }

        private static async Task WriteAllEmbeddingsToFile(string filename, List<object> embeddingsList, string embedlocation)
        {
            // Create an object that includes the list of embeddings and the source file name
            var outputObject = new
            {
                sourceFileName = filename,
                embeddings = embeddingsList
            };

            // Write the embeddings JSON file with the filename in Embeddings folder
            string jsonFileName = $"embed_{filename}.json";
            string jsonFilePath = Path.Combine(embedlocation , jsonFileName);

            string json = JsonSerializer.Serialize(outputObject);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, json);
            Console.WriteLine(json);
        }

    }
}
