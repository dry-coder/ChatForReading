namespace Client
{
    partial class Friendreq
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CancelmuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lv_putmureq = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lv_mureq = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CancelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 34);
            // 
            // CancelToolStripMenuItem
            // 
            this.CancelToolStripMenuItem.Name = "CancelToolStripMenuItem";
            this.CancelToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.CancelToolStripMenuItem.Text = "取消请求";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CancelmuToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 34);
            // 
            // CancelmuToolStripMenuItem
            // 
            this.CancelmuToolStripMenuItem.Name = "CancelmuToolStripMenuItem";
            this.CancelmuToolStripMenuItem.Size = new System.Drawing.Size(152, 30);
            this.CancelmuToolStripMenuItem.Text = "取消请求";
            this.CancelmuToolStripMenuItem.Click += new System.EventHandler(this.CancelmuToolStripMenuItem_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lv_putmureq);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(361, 530);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "发出的入群申请";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lv_putmureq
            // 
            this.lv_putmureq.HideSelection = false;
            this.lv_putmureq.Location = new System.Drawing.Point(6, 6);
            this.lv_putmureq.Margin = new System.Windows.Forms.Padding(4);
            this.lv_putmureq.Name = "lv_putmureq";
            this.lv_putmureq.Size = new System.Drawing.Size(344, 484);
            this.lv_putmureq.TabIndex = 0;
            this.lv_putmureq.UseCompatibleStateImageBehavior = false;
            this.lv_putmureq.View = System.Windows.Forms.View.Details;
            this.lv_putmureq.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lv_putmureq_MouseClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lv_mureq);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(361, 530);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "入群申请";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lv_mureq
            // 
            this.lv_mureq.HideSelection = false;
            this.lv_mureq.Location = new System.Drawing.Point(6, 6);
            this.lv_mureq.Margin = new System.Windows.Forms.Padding(4);
            this.lv_mureq.Name = "lv_mureq";
            this.lv_mureq.Size = new System.Drawing.Size(344, 484);
            this.lv_mureq.TabIndex = 0;
            this.lv_mureq.UseCompatibleStateImageBehavior = false;
            this.lv_mureq.View = System.Windows.Forms.View.Details;
            this.lv_mureq.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_mureq_MouseDoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(18, 18);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(369, 562);
            this.tabControl1.TabIndex = 1;
            // 
            // Friendreq
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 612);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Friendreq";
            this.Text = "Friendreq";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Friendreq_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Friendreq_FormClosed);
            this.Load += new System.EventHandler(this.Friendreq_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem CancelToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem CancelmuToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListView lv_putmureq;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView lv_mureq;
        private System.Windows.Forms.TabControl tabControl1;
    }
}