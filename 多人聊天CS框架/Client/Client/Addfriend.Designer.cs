namespace Client
{
    partial class Addfriend
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.bt_musend = new System.Windows.Forms.Button();
            this.cb_mugroup = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_GID = new System.Windows.Forms.TextBox();
            this.lb_mustate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(18, 18);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(429, 327);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.bt_musend);
            this.tabPage2.Controls.Add(this.cb_mugroup);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.tb_GID);
            this.tabPage2.Controls.Add(this.lb_mustate);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(421, 295);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "添加群组";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // bt_musend
            // 
            this.bt_musend.Location = new System.Drawing.Point(234, 208);
            this.bt_musend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_musend.Name = "bt_musend";
            this.bt_musend.Size = new System.Drawing.Size(112, 34);
            this.bt_musend.TabIndex = 5;
            this.bt_musend.Text = "提交";
            this.bt_musend.UseVisualStyleBackColor = true;
            this.bt_musend.Click += new System.EventHandler(this.bt_musend_Click);
            // 
            // cb_mugroup
            // 
            this.cb_mugroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_mugroup.FormattingEnabled = true;
            this.cb_mugroup.Location = new System.Drawing.Point(147, 142);
            this.cb_mugroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cb_mugroup.Name = "cb_mugroup";
            this.cb_mugroup.Size = new System.Drawing.Size(198, 26);
            this.cb_mugroup.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 116);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "想加入的分组:";
            // 
            // tb_GID
            // 
            this.tb_GID.Location = new System.Drawing.Point(147, 56);
            this.tb_GID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tb_GID.Name = "tb_GID";
            this.tb_GID.Size = new System.Drawing.Size(198, 28);
            this.tb_GID.TabIndex = 2;
            // 
            // lb_mustate
            // 
            this.lb_mustate.AutoSize = true;
            this.lb_mustate.Location = new System.Drawing.Point(189, 33);
            this.lb_mustate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_mustate.Name = "lb_mustate";
            this.lb_mustate.Size = new System.Drawing.Size(0, 18);
            this.lb_mustate.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 33);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "输入想加入的群组ID:";
            // 
            // Addfriend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 370);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Addfriend";
            this.Text = "Addfriend";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Addfriend_FormClosed);
            this.Load += new System.EventHandler(this.Addfriend_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button bt_musend;
        private System.Windows.Forms.ComboBox cb_mugroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_GID;
        private System.Windows.Forms.Label lb_mustate;
        private System.Windows.Forms.Label label3;
    }
}