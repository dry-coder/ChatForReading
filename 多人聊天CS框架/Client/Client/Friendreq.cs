using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;

namespace Client
{
    public partial class Friendreq : Form
    {
        public bool iswork { get; set; }
        public BinaryWriter Bw { get; set; }
        public static List<Mutualreqitem> muformList = new List<Mutualreqitem>();
        public Friendreq()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //群组申请
            lv_mureq.Columns.Add("UID", 45, HorizontalAlignment.Left);
            lv_mureq.Columns.Add("昵称", 60, HorizontalAlignment.Left);
            lv_mureq.Columns.Add("GID", 45, HorizontalAlignment.Left);
            lv_mureq.Columns.Add("群名", 60, HorizontalAlignment.Left);
            lv_mureq.Columns.Add("请求状态", 60, HorizontalAlignment.Left);
            //发送的群组申请
            lv_putmureq.Columns.Add("GID", 45, HorizontalAlignment.Left);
            lv_putmureq.Columns.Add("群名", 60, HorizontalAlignment.Left);
            lv_putmureq.Columns.Add("请求状态", 60, HorizontalAlignment.Left);
        }
        private void Friendreq_Load(object sender, EventArgs e)
        {
            addmureqitem();
            addputmureqitem();
        }

        public void addmureqitem()
        {
            for (int i = 0; i < list.mureqList.Count; i++)
            {

                ListViewItem lvitem = new ListViewItem();
                lvitem.ImageIndex = 0;
                lvitem.Text = list.mureqList[i].UID;
                lvitem.SubItems.Add(list.mureqList[i].Username);
                lvitem.SubItems.Add(list.mureqList[i].GID);
                lvitem.SubItems.Add(list.mureqList[i].Groupname);
                lvitem.SubItems.Add("未处理");
                lvitem.Tag = 0;
                lv_mureq.Items.Add(lvitem);
            }
        }
        public void addputmureqitem()
        {
            for (int i = 0; i < list.putmureqList.Count; i++)
            {

                ListViewItem lvitem = new ListViewItem();
                lvitem.ImageIndex = 0;
                lvitem.Text = list.putmureqList[i].GID;
                lvitem.SubItems.Add(list.putmureqList[i].Groupname);
                lvitem.SubItems.Add("未处理");
                lvitem.Tag = 0;
                lv_putmureq.Items.Add(lvitem);
            }
        }
        public void reloadmureqitem()
        {
            //MessageBox.Show("trytoclear");
            lv_mureq.Items.Clear();
            addmureqitem();
        }
        public void reloadputmureqitem()
        {
            //MessageBox.Show("trytoclear");
            lv_putmureq.Items.Clear();
            addputmureqitem();
        }
        private void lv_mureq_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.lv_mureq.SelectedItems.Count > 0)
            {
                ListViewItem lvitem = this.lv_mureq.SelectedItems[0];
                string UID = lvitem.Text;
                string GID = lvitem.SubItems[2].Text;
                //string toips = lvitem.Tag.ToString();
                Mutualreqitem f = isHavemureqitem(GID,UID);
                if (f != null)
                {
                    f.Focus();
                }
                else
                {
                    Mutualreqitem muitm = new Mutualreqitem();
                    muitm.UID = UID;
                    muitm.GID = GID;
                    muitm.Bw = Bw;
                    muitm.toname = lvitem.SubItems[1].Text;
                    muitm.groupname = lvitem.SubItems[3].Text;
                    muformList.Add(muitm);
                    muitm.Show();
                }
            }
        }
        private void Friendreq_FormClosed(object sender, FormClosedEventArgs e)
        {
            list.RemoveFriendreq();
        }

        
        private Mutualreqitem isHavemureqitem(String GID,String UID)
        {
            foreach (Mutualreqitem muitm in muformList)
            {
                if (muitm.UID == UID && muitm.GID==GID)
                    return muitm;
            }
            return null;
        }
        public static void removeMutualdreqitem(String GID,String UID)
        {
            foreach (Mutualreqitem muitm in muformList)
            {
                if (muitm.UID == UID && muitm.GID == GID)
                {
                    muformList.Remove(muitm);
                    return;
                }
            }
        }

        private void Friendreq_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*for (int i = 0; i < frdformList.Count; i++)
            {

            }*/
        }

        private void lv_putmureq_MouseClick(object sender, MouseEventArgs e)
        {
            lv_putmureq.MultiSelect = false;
            if (e.Button == MouseButtons.Right)
            {
                string GID = lv_putmureq.SelectedItems[0].Text;
                Point p = new Point(e.X, e.Y);
                contextMenuStrip2.Show(lv_putmureq, p);
            }
        }
        private void CancelmuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.lv_putmureq.SelectedItems.Count == 0)
                return;
            ListViewItem lvitm = this.lv_putmureq.SelectedItems[0];
            Bw.Write("cancelmureq#" + lvitm.SubItems[0].Text);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
