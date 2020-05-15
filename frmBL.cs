using System;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace BlackList
{
    public partial class frmBL : Form
    {

        private IrcClient IRC = null;
        private System.Timers.Timer MT = null;
        private volatile bool IsConn = false;

        public frmBL()
        {
            InitializeComponent();

            lbx_Chats.DrawMode = DrawMode.OwnerDrawFixed;
            lbx_Chats.MeasureItem += (s, e) => { };
            lbx_Chats.DrawItem += Lbx_Chats_DrawItem; ;
        }

        private void Lbx_Chats_DrawItem(object sender, DrawItemEventArgs e)
        {
            // 如果该项目是选定的项目，则绘制填充为蓝色的矩形。 当State属性和DrawItemState.Selected属性的按位“与”为true时，将选择该项目。
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
            }
            else
            {
                // 否则，绘制米色填充的矩形。
                e.Graphics.FillRectangle(Brushes.Black, e.Bounds);
            }

            // 在每个项目周围绘制一个蓝色的矩形。
            e.Graphics.DrawRectangle(Pens.Blue, e.Bounds);

            var item = (ChatMsg)lbx_Chats.Items[e.Index];

            // 在项目中绘制文本。
            e.Graphics.DrawString(lbx_Chats.Items[e.Index].ToString(),
                this.Font, item.Color, e.Bounds.X, e.Bounds.Y);

            // 在所选项目周围绘制焦点矩形。
            e.DrawFocusRectangle();
        }

        private void frmBL_Load(object sender, System.EventArgs e)
        {
            if (IRC == null)
            {
                MT = new System.Timers.Timer();
                MT.Interval = 2500;
                MT.Elapsed += MT_Elapsed;

                IRC = new IrcClient("", new IrcUser(Program.Name), false);

                IRC.ConnectionComplete += IRC_ConnectionComplete;

                IRC.UserJoinedChannel += IRC_UserJoinedChannel;
                IRC.UserQuit += IRC_UserQuit;

                IRC.ChannelMessageRecieved += IRC_ChannelMessageRecieved;
                IRC.PrivateMessageRecieved += IRC_PrivateMessageRecieved;

                IRC.WhoIsReceived += IRC_WhoIsReceived;

                IRC.NetworkError += IRC_NetworkError;
                IRC.Error += IRC_Error;

                IRC.ChatLogRep += IRC_ChatLogRep;

                IRC.ConnectToServer();
            }
        }

        private void MT_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsConn == true)
            {
                IRC.SendRawMessage($"PRIVMSG #cncnet-mo :@baba666脑瘫玩意你妈死了你知道吗?  你慢慢打字，程序不和你一般见识 {new Random().Next(10, 99)}");
            }
        }

        private void IRC_WhoIsReceived(object sender, Events.WhoIsReceivedEventArgs e)
        {
            Action act = delegate () {
                tbo_Name.Text = e.WhoIsResponse.User.Nick;
                tbo_ID.Text = e.WhoIsResponse.User.User;
                tbo_IP.Text = e.WhoIsResponse.User.Hostname;
            };
            this.BeginInvoke(act);
        }

        private void IRC_PrivateMessageRecieved(object sender, Events.PrivateMessageEventArgs e)
        {
            if (e.PrivateMessage.IsChannelMessage == false)
            {
                var msg = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Priv] {e.PrivateMessage.User.Nick}:{e.PrivateMessage.Message}";
                if ((msg.Length / 52) == 0)
                {
                    Action actx = delegate () { lbx_Chats.Items.Add(new ChatMsg(Brushes.Red, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg}")); };
                    this.BeginInvoke(actx);
                }
                else
                {
                    Action act = delegate () {
                        var fo2 = msg.Length / 52;
                        for (int i = 0; i < fo2; i++)
                        {
                            if (i == fo2)
                            {
                                lbx_Chats.Items.Add(new ChatMsg(Brushes.Red, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg.Substring(i * 52)}"));
                                break;
                            }
                            else
                            {
                                lbx_Chats.Items.Add(new ChatMsg(Brushes.Red, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg.Substring(i * 52, 52)}"));
                            }
                        }
                    };
                    this.BeginInvoke(act);
                }
            }
        }

        private void IRC_ChannelMessageRecieved(object sender, Events.PrivateMessageEventArgs e)
        {
            if (e.PrivateMessage.IsChannelMessage == true)
            {
                var msg = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Chat] {e.PrivateMessage.User.Nick}:{e.PrivateMessage.Message.Substring(3)}";
                var col = IrcClient.Colors[Convert.ToInt32(e.PrivateMessage.Message.Substring(1, 2))];

                if ((msg.Length / 52) == 0)
                {
                    Action actx = delegate () { lbx_Chats.Items.Add(new ChatMsg(col.Color, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg}")); };
                    this.BeginInvoke(actx);
                }
                else
                {
                    Action act = delegate () {
                        var fo2 = msg.Length / 52;
                        for (int i = 0; i < fo2; i++)
                        {
                            if (i == fo2)
                            {
                                lbx_Chats.Items.Add(new ChatMsg(col.Color, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg.Substring(i * 52)}"));
                                break;
                            }
                            else
                            {
                                lbx_Chats.Items.Add(new ChatMsg(col.Color, e.PrivateMessage.User.Nick, e.PrivateMessage.User.User, e.PrivateMessage.User.Hostname, $"{msg.Substring(i * 52, 52)}"));
                            }
                        }
                    };
                    this.BeginInvoke(act);
                }
            }
        }

        private void IRC_UserJoinedChannel(object sender, Events.ChannelUserEventArgs e)
        {
            Action act = delegate () {
                lbx_Chats.Items.Add(new ChatMsg(e.User.Nick, e.User.Account, e.User.Hostname,$"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Join] 用户 {e.User.Nick} 上线"));
            };
            this.BeginInvoke(act);
        }

        private void IRC_UserQuit(object sender, Events.UserEventArgs e)
        {
            Action act = delegate () {
                lbx_Chats.Items.Add(new ChatMsg(e.User.Nick, e.User.Account, e.User.Hostname, $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Leav] 用户 {e.User.Nick} 下线"));
            };
            this.BeginInvoke(act);
        }

        private void IRC_ConnectionComplete(object sender, EventArgs e)
        {
            IRC.JoinChannel("#cncnet-mo", "ra1-derp");
            new Thread(ProcessUserList).Start();
        }

        private void ProcessUserList()
        {
            Thread.Sleep(3000);
            Action act = delegate ()
            {
                lbx_UserList.Items.Clear();
                var gul = IRC.Users.GetUserList;
                for (int i = 0; i < gul.Count; i++)
                {
                    lbx_UserList.Items.Add(gul[i].Nick);
                }

                tbo_Input.ReadOnly = false;
                lbx_BlackList.SelectionMode = SelectionMode.One;
                lbx_Chats.SelectionMode = SelectionMode.One;
                lbx_UserList.SelectionMode = SelectionMode.One;
                this.lbx_BlackList.MouseClick += new MouseEventHandler(this.ALL_MouseClick);
                this.lbx_UserList.MouseClick += new MouseEventHandler(this.ALL_MouseClick);
                this.lbx_Chats.MouseClick += new MouseEventHandler(this.ALL_MouseClick);
                this.lbx_BlackList.MouseDoubleClick += new MouseEventHandler(this.ALL_MouseDoubleClick);
                this.lbx_UserList.MouseDoubleClick += new MouseEventHandler(this.ALL_MouseDoubleClick);
                this.lbx_Chats.MouseDoubleClick += new MouseEventHandler(this.ALL_MouseDoubleClick);

                IsConn = true;
                //MT.Start();
            };
            this.BeginInvoke(act);
        }

        private void IRC_ChatLogRep(object sender, string e)
        {
            Action act = delegate () {
                lbx_Chats.Items.Add(new ChatMsg($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Info] {e}"));
            };
            this.BeginInvoke(act);
        }

        private void IRC_Error(object sender, Events.ErrorEventArgs e)
        {
            Action act = delegate () {
                lbx_Chats.Items.Add(new ChatMsg(Brushes.Red, $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Warn] {e.Error.Message}"));
                IsConn = false;
            };
            this.BeginInvoke(act);
            IRC.ConnectToServer();
        }

        private void IRC_NetworkError(object sender, Events.SocketErrorEventArgs e)
        {
            Action act = delegate () {
                lbx_Chats.Items.Add(new ChatMsg(Brushes.Red, $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] [Warn] {e.SocketError.ToString()}"));
                IsConn = false;
            };
            this.BeginInvoke(act);
        }

        private void ALL_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsConn == true)
            {
                var obj = (ListBox)sender;
                switch (obj.Name)
                {
                    case "lbx_Chats":
                        var msg = (ChatMsg)obj.SelectedItem;
                        tbo_Name.Text = msg.Name;
                        tbo_ID.Text = msg.ID;
                        tbo_IP.Text = msg.IP;
                        break;
                    case "lbx_UserList":
                        IRC.WhoIs(lbx_UserList.SelectedItem.ToString());
                        break;
                    case "lbx_BlackList":
                        break;
                    default:
                        break;
                }
            }
        }

        private void ALL_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IsConn == true)
            {
                var obj = (ListBox)sender;
                switch (obj.Name)
                {
                    case "lbx_Chats":
                        break;
                    case "lbx_UserList":
                        break;
                    case "lbx_BlackList":
                        break;
                    default:
                        break;
                }
            }
        }

        private void tbo_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tbo_Input.Text.Substring(0,3) == "say ")
                {

                }
                //IRC.WhoIs(tbo_Input.Text);
                //IRC.SendRawMessage(tbo_Input.Text);
                //IRC.SendIrcMessage(new IrcMessage($"PRIVMSG #cncnet-mo :\u000306 {tbo_Input.Text}{new Random().Next(0, 9)}"));
                //MList.Add($"PRIVMSG #cncnet-mo :\u000306 {tbo_Input.Text}{new Random().Next(0, 9)}");
                IRC.SendRawMessage($"PRIVMSG #cncnet-mo :\u000306 {tbo_Input.Text}{new Random().Next(0, 9)} \r\n");
                tbo_Input.Text = "";
            }
            //new Thread(Fuck).Start();
        }

        private void Fuck()
        {
            while (true)
            {
                
                Thread.Sleep(-1);
            }
        }
    }
}