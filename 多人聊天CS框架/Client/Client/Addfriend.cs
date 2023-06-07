using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Addfriend : Form
    {
        public bool iswork { get; set; }
        public BinaryWriter Bw { get; set; }
        public Addfriend()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Addfriend_FormClosed(object sender, FormClosedEventArgs e)
        {
            list.RemoveAddfrd();
        }
        private void Addfriend_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < list.muGroupList.Count; i++)
            {
                cb_mugroup.Items.Add(list.muGroupList[i]);
            }
        }

        public void get_true()
        {
            MessageBox.Show("添加成功");
            //Bw.Write("getputreq");
            this.Close();
        }

        private void bt_musend_Click(object sender, EventArgs e)
        {
            if (tb_GID.Text == "")
            {
                lb_mustate.Text = "GID不能为空";
                return;
            }
            if (cb_mugroup.Text == "")
            {
                lb_mustate.Text = "分组不能为空";
                return;
            }
            String sndmsg = "addgroup#";
            sndmsg += tb_GID.Text;
            sndmsg += "#";
            sndmsg += cb_mugroup.Text;
            Bw.Write(sndmsg);
        }
        public void get_mumsg(String msg)
        {
            lb_mustate.Text = msg;
        }

        public void get_mutrue()
        {
            MessageBox.Show("添加成功");
            //Bw.Write("getputreq");
            this.Close();
        }
    }
}
