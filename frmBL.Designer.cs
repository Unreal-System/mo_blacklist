namespace BlackList
{
    partial class frmBL
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
            this.lbx_BlackList = new System.Windows.Forms.ListBox();
            this.lbx_UserList = new System.Windows.Forms.ListBox();
            this.gbx_BlackList = new System.Windows.Forms.GroupBox();
            this.gbx_UserList = new System.Windows.Forms.GroupBox();
            this.tlp_Main = new System.Windows.Forms.TableLayoutPanel();
            this.gbx_Main = new System.Windows.Forms.GroupBox();
            this.tlp_Gen = new System.Windows.Forms.TableLayoutPanel();
            this.lbx_Chats = new System.Windows.Forms.ListBox();
            this.tbo_Input = new System.Windows.Forms.TextBox();
            this.tlp_Info = new System.Windows.Forms.TableLayoutPanel();
            this.tbo_ID = new System.Windows.Forms.TextBox();
            this.tbo_IP = new System.Windows.Forms.TextBox();
            this.tbo_Name = new System.Windows.Forms.TextBox();
            this.tbo_Names = new System.Windows.Forms.TextBox();
            this.lbl_ID = new System.Windows.Forms.Label();
            this.lbl_IP = new System.Windows.Forms.Label();
            this.lbl_Names = new System.Windows.Forms.Label();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.gbx_BlackList.SuspendLayout();
            this.gbx_UserList.SuspendLayout();
            this.tlp_Main.SuspendLayout();
            this.gbx_Main.SuspendLayout();
            this.tlp_Gen.SuspendLayout();
            this.tlp_Info.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbx_BlackList
            // 
            this.lbx_BlackList.BackColor = System.Drawing.Color.Black;
            this.lbx_BlackList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbx_BlackList.ForeColor = System.Drawing.Color.White;
            this.lbx_BlackList.FormattingEnabled = true;
            this.lbx_BlackList.ItemHeight = 12;
            this.lbx_BlackList.Location = new System.Drawing.Point(3, 17);
            this.lbx_BlackList.Name = "lbx_BlackList";
            this.lbx_BlackList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbx_BlackList.Size = new System.Drawing.Size(144, 535);
            this.lbx_BlackList.TabIndex = 0;
            // 
            // lbx_UserList
            // 
            this.lbx_UserList.BackColor = System.Drawing.Color.Black;
            this.lbx_UserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbx_UserList.ForeColor = System.Drawing.Color.White;
            this.lbx_UserList.FormattingEnabled = true;
            this.lbx_UserList.ItemHeight = 12;
            this.lbx_UserList.Location = new System.Drawing.Point(3, 17);
            this.lbx_UserList.Name = "lbx_UserList";
            this.lbx_UserList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbx_UserList.Size = new System.Drawing.Size(146, 535);
            this.lbx_UserList.TabIndex = 1;
            // 
            // gbx_BlackList
            // 
            this.gbx_BlackList.Controls.Add(this.lbx_BlackList);
            this.gbx_BlackList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbx_BlackList.Location = new System.Drawing.Point(3, 3);
            this.gbx_BlackList.Name = "gbx_BlackList";
            this.gbx_BlackList.Size = new System.Drawing.Size(150, 555);
            this.gbx_BlackList.TabIndex = 2;
            this.gbx_BlackList.TabStop = false;
            this.gbx_BlackList.Text = "黑名单列表";
            // 
            // gbx_UserList
            // 
            this.gbx_UserList.Controls.Add(this.lbx_UserList);
            this.gbx_UserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbx_UserList.Location = new System.Drawing.Point(629, 3);
            this.gbx_UserList.Name = "gbx_UserList";
            this.gbx_UserList.Size = new System.Drawing.Size(152, 555);
            this.gbx_UserList.TabIndex = 4;
            this.gbx_UserList.TabStop = false;
            this.gbx_UserList.Text = "在线用户";
            // 
            // tlp_Main
            // 
            this.tlp_Main.ColumnCount = 3;
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlp_Main.Controls.Add(this.gbx_Main, 1, 0);
            this.tlp_Main.Controls.Add(this.gbx_BlackList, 0, 0);
            this.tlp_Main.Controls.Add(this.gbx_UserList, 2, 0);
            this.tlp_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Main.Location = new System.Drawing.Point(0, 0);
            this.tlp_Main.Name = "tlp_Main";
            this.tlp_Main.RowCount = 1;
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Main.Size = new System.Drawing.Size(784, 561);
            this.tlp_Main.TabIndex = 6;
            // 
            // gbx_Main
            // 
            this.gbx_Main.Controls.Add(this.tlp_Gen);
            this.gbx_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbx_Main.Location = new System.Drawing.Point(159, 3);
            this.gbx_Main.Name = "gbx_Main";
            this.gbx_Main.Size = new System.Drawing.Size(464, 555);
            this.gbx_Main.TabIndex = 7;
            this.gbx_Main.TabStop = false;
            // 
            // tlp_Gen
            // 
            this.tlp_Gen.ColumnCount = 1;
            this.tlp_Gen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Gen.Controls.Add(this.lbx_Chats, 0, 1);
            this.tlp_Gen.Controls.Add(this.tbo_Input, 0, 2);
            this.tlp_Gen.Controls.Add(this.tlp_Info, 0, 0);
            this.tlp_Gen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Gen.Location = new System.Drawing.Point(3, 17);
            this.tlp_Gen.Name = "tlp_Gen";
            this.tlp_Gen.RowCount = 3;
            this.tlp_Gen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tlp_Gen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Gen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlp_Gen.Size = new System.Drawing.Size(458, 535);
            this.tlp_Gen.TabIndex = 0;
            // 
            // lbx_Chats
            // 
            this.lbx_Chats.BackColor = System.Drawing.Color.Black;
            this.lbx_Chats.DisplayMember = "Msg";
            this.lbx_Chats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbx_Chats.ForeColor = System.Drawing.Color.White;
            this.lbx_Chats.FormattingEnabled = true;
            this.lbx_Chats.ItemHeight = 12;
            this.lbx_Chats.Location = new System.Drawing.Point(3, 108);
            this.lbx_Chats.Name = "lbx_Chats";
            this.lbx_Chats.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbx_Chats.Size = new System.Drawing.Size(452, 399);
            this.lbx_Chats.TabIndex = 3;
            this.lbx_Chats.ValueMember = "Msg";
            // 
            // tbo_Input
            // 
            this.tbo_Input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbo_Input.Location = new System.Drawing.Point(3, 513);
            this.tbo_Input.MaxLength = 198;
            this.tbo_Input.Multiline = true;
            this.tbo_Input.Name = "tbo_Input";
            this.tbo_Input.ReadOnly = true;
            this.tbo_Input.Size = new System.Drawing.Size(452, 19);
            this.tbo_Input.TabIndex = 2;
            this.tbo_Input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbo_Input_KeyDown);
            // 
            // tlp_Info
            // 
            this.tlp_Info.ColumnCount = 2;
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlp_Info.Controls.Add(this.tbo_ID, 1, 1);
            this.tlp_Info.Controls.Add(this.tbo_IP, 1, 2);
            this.tlp_Info.Controls.Add(this.tbo_Name, 1, 0);
            this.tlp_Info.Controls.Add(this.tbo_Names, 1, 3);
            this.tlp_Info.Controls.Add(this.lbl_ID, 0, 1);
            this.tlp_Info.Controls.Add(this.lbl_IP, 0, 2);
            this.tlp_Info.Controls.Add(this.lbl_Names, 0, 3);
            this.tlp_Info.Controls.Add(this.lbl_Name, 0, 0);
            this.tlp_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Info.Location = new System.Drawing.Point(3, 3);
            this.tlp_Info.Name = "tlp_Info";
            this.tlp_Info.RowCount = 6;
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tlp_Info.Size = new System.Drawing.Size(452, 99);
            this.tlp_Info.TabIndex = 8;
            // 
            // tbo_ID
            // 
            this.tbo_ID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbo_ID.Location = new System.Drawing.Point(45, 25);
            this.tbo_ID.Name = "tbo_ID";
            this.tbo_ID.ReadOnly = true;
            this.tbo_ID.Size = new System.Drawing.Size(404, 21);
            this.tbo_ID.TabIndex = 4;
            // 
            // tbo_IP
            // 
            this.tbo_IP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbo_IP.Location = new System.Drawing.Point(45, 47);
            this.tbo_IP.Name = "tbo_IP";
            this.tbo_IP.ReadOnly = true;
            this.tbo_IP.Size = new System.Drawing.Size(404, 21);
            this.tbo_IP.TabIndex = 5;
            // 
            // tbo_Name
            // 
            this.tbo_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbo_Name.Location = new System.Drawing.Point(45, 3);
            this.tbo_Name.Name = "tbo_Name";
            this.tbo_Name.ReadOnly = true;
            this.tbo_Name.Size = new System.Drawing.Size(404, 21);
            this.tbo_Name.TabIndex = 3;
            // 
            // tbo_Names
            // 
            this.tbo_Names.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbo_Names.Location = new System.Drawing.Point(45, 69);
            this.tbo_Names.Name = "tbo_Names";
            this.tbo_Names.ReadOnly = true;
            this.tbo_Names.Size = new System.Drawing.Size(404, 21);
            this.tbo_Names.TabIndex = 7;
            // 
            // lbl_ID
            // 
            this.lbl_ID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ID.Font = new System.Drawing.Font("宋体", 9F);
            this.lbl_ID.ForeColor = System.Drawing.Color.Black;
            this.lbl_ID.Location = new System.Drawing.Point(3, 22);
            this.lbl_ID.Name = "lbl_ID";
            this.lbl_ID.Size = new System.Drawing.Size(36, 22);
            this.lbl_ID.TabIndex = 1;
            this.lbl_ID.Text = " ID :";
            // 
            // lbl_IP
            // 
            this.lbl_IP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_IP.Font = new System.Drawing.Font("宋体", 9F);
            this.lbl_IP.ForeColor = System.Drawing.Color.Black;
            this.lbl_IP.Location = new System.Drawing.Point(3, 44);
            this.lbl_IP.Name = "lbl_IP";
            this.lbl_IP.Size = new System.Drawing.Size(36, 22);
            this.lbl_IP.TabIndex = 2;
            this.lbl_IP.Text = "地址:";
            // 
            // lbl_Names
            // 
            this.lbl_Names.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Names.Font = new System.Drawing.Font("宋体", 9F);
            this.lbl_Names.ForeColor = System.Drawing.Color.Black;
            this.lbl_Names.Location = new System.Drawing.Point(3, 66);
            this.lbl_Names.Name = "lbl_Names";
            this.lbl_Names.Size = new System.Drawing.Size(36, 22);
            this.lbl_Names.TabIndex = 6;
            this.lbl_Names.Text = "马甲:";
            // 
            // lbl_Name
            // 
            this.lbl_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_Name.Font = new System.Drawing.Font("宋体", 9F);
            this.lbl_Name.ForeColor = System.Drawing.Color.Black;
            this.lbl_Name.Location = new System.Drawing.Point(3, 0);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(36, 22);
            this.lbl_Name.TabIndex = 0;
            this.lbl_Name.Text = "昵称:";
            // 
            // frmBL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tlp_Main);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmBL";
            this.Text = "喵喵喵";
            this.Load += new System.EventHandler(this.frmBL_Load);
            this.gbx_BlackList.ResumeLayout(false);
            this.gbx_UserList.ResumeLayout(false);
            this.tlp_Main.ResumeLayout(false);
            this.gbx_Main.ResumeLayout(false);
            this.tlp_Gen.ResumeLayout(false);
            this.tlp_Gen.PerformLayout();
            this.tlp_Info.ResumeLayout(false);
            this.tlp_Info.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbx_BlackList;
        private System.Windows.Forms.ListBox lbx_UserList;
        private System.Windows.Forms.GroupBox gbx_BlackList;
        private System.Windows.Forms.GroupBox gbx_UserList;
        private System.Windows.Forms.TableLayoutPanel tlp_Main;
        private System.Windows.Forms.GroupBox gbx_Main;
        private System.Windows.Forms.TableLayoutPanel tlp_Gen;
        private System.Windows.Forms.TableLayoutPanel tlp_Info;
        private System.Windows.Forms.TextBox tbo_ID;
        private System.Windows.Forms.TextBox tbo_IP;
        private System.Windows.Forms.TextBox tbo_Name;
        private System.Windows.Forms.TextBox tbo_Names;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label lbl_ID;
        private System.Windows.Forms.Label lbl_IP;
        private System.Windows.Forms.Label lbl_Names;
        private System.Windows.Forms.ListBox lbx_Chats;
        private System.Windows.Forms.TextBox tbo_Input;
    }
}