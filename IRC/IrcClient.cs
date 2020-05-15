using BlackList.Events;
using BlackList.Handlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace BlackList
{
    /// <summary>
    /// An IRC client.
    /// </summary>
    public sealed partial class IrcClient
    {
        /// <summary>
        /// A raw IRC message handler.
        /// </summary>
        public delegate void MessageHandler(IrcClient client, IrcMessage message);
        private Dictionary<string, MessageHandler> Handlers { get; set; }

        /// <summary>
        /// Sets a custom handler for an IRC message. This applies to the low level IRC protocol,
        /// not for private messages.
        /// </summary>
        public void SetHandler(string message, MessageHandler handler)
        {
#if DEBUG
            // This is the default behavior if 3rd parties want to handle certain messages themselves
            // However, if it happens from our own code, we probably did something wrong
            if (Handlers.ContainsKey(message.ToUpper()))
                Console.WriteLine("Warning: {0} handler has been overwritten", message);
#endif
            message = message.ToUpper();
            Handlers[message] = handler;
        }

        internal static DateTime DateTimeFromIrcTime(int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        internal static Random RandomNumber { get; private set; }

        private const int ReadBufferLength = 1024;

        private byte[] ReadBuffer { get; set; }
        private int ReadBufferIndex { get; set; }
        private string ServerHostname { get; set; }
        private int ServerPort { get; set; }
        private Timer PingTimer { get; set; }
        private Socket Socket { get; set; }
        private ConcurrentQueue<string> WriteQueue { get; set; }
        private bool IsWriting { get; set; }

        internal RequestManager RequestManager { get; set; }

        internal string ServerNameFromPing { get; set; }

        /// <summary>
        /// The address this client is connected to, or will connect to. Setting this
        /// after the client is connected will not cause a reconnect.
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return ServerHostname + ":" + ServerPort;
            }
            internal set
            {
                string[] parts = value.Split(':');
                if (parts.Length > 2 || parts.Length == 0)
                    throw new FormatException("Server address is not in correct format ('hostname:port')");
                ServerHostname = parts[0];
                if (parts.Length > 1)
                    ServerPort = int.Parse(parts[1]);
                else
                    ServerPort = 6667;
            }
        }

        /// <summary>
        /// The low level TCP stream for this client.
        /// </summary>
        public Stream NetworkStream { get; set; }
        /// <summary>
        /// If true, SSL will be used to connect.
        /// </summary>
        public bool UseSSL { get; private set; }
        /// <summary>
        /// If true, invalid SSL certificates are ignored.
        /// </summary>
        public bool IgnoreInvalidSSL { get; set; }
        /// <summary>
        /// The character encoding to use for the connection. Defaults to UTF-8.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// The user this client is logged in as.
        /// </summary>
        /// <value>The user.</value>
        public IrcUser User { get; set; }
        /// <summary>
        /// The channels this user is joined to.
        /// </summary>
        public ChannelCollection Channels { get; private set; }
        /// <summary>
        /// Settings that control the behavior of CnCNET.
        /// </summary>
        public ClientSettings Settings { get; set; }
        /// <summary>
        /// Information about the server we are connected to. Servers may not send us this information,
        /// but it's required for CnCNET to function, so by default this is a guess. Handle
        /// IrcClient.ServerInfoRecieved if you'd like to know when it's populated with real information.
        /// </summary>
        public ServerInfo ServerInfo { get; set; }
        /// <summary>
        /// A string to prepend to all PRIVMSGs sent. Many IRC bots prefix their messages with \u200B, to
        /// indicate to other bots that you are a bot.
        /// </summary>
        public string PrivmsgPrefix { get; set; }
        /// <summary>
        /// A list of users on this network that we are aware of.
        /// </summary>
        public UserPool Users { get; set; }
        /// <summary>
        /// A list of capabilities supported by the library, along with enabled and disabled capabilities
        /// after negotiating with the server.
        /// </summary>
        public CapabilityPool Capabilities { get; set; }
        /// <summary>
        /// Set to true when the client is negotiating IRC capabilities with the server.
        /// If set to False, capability negotiation is finished.
        /// </summary>
        public bool IsNegotiatingCapabilities { get; internal set; }
        /// <summary>
        /// Set to True when the client is authenticating with SASL.
        /// If set to False, SASL authentication is finished.
        /// </summary>
        public bool IsAuthenticatingSasl { get; internal set; }

        /// <summary>
        /// Creates a new IRC client, but will not connect until ConnectAsync is called.
        /// </summary>
        /// <param name="serverAddress">Server address including port in the form of "hostname:port".</param>
        /// <param name="user">The IRC user to connect as.</param>
        /// <param name="useSSL">Connect with SSL if true.</param>
        public IrcClient(string serverAddress, IrcUser user, bool useSSL = false)
        {
            if (serverAddress == null) throw new ArgumentNullException("serverAddress");
            if (user == null) throw new ArgumentNullException("user");

            User = user;
            ServerAddress = serverAddress;
            Encoding = Encoding.UTF8;
            Settings = new ClientSettings();
            Handlers = new Dictionary<string, MessageHandler>();
            MessageHandlers.RegisterDefaultHandlers(this);
            RequestManager = new RequestManager();
            UseSSL = useSSL;
            WriteQueue = new ConcurrentQueue<string>();
            ServerInfo = new ServerInfo();
            PrivmsgPrefix = "";
            Channels = User.Channels = new ChannelCollection(this);
            Users = new UserPool();
            Users.Add(User); // Add self to user pool
            Capabilities = new CapabilityPool();

            // List of supported capabilities
            Capabilities.AddRange(new string[] {
                "server-time", "multi-prefix", "cap-notify", "znc.in/server-time", "znc.in/server-time-iso",
                "account-notify", "chghost", "userhost-in-names", "sasl"
            });

            IsNegotiatingCapabilities = false;
            IsAuthenticatingSasl = false;

            RandomNumber = new Random();
        }

        private List<int> failedServerIds = new List<int>();
        private static readonly IList<Server> Servers = new List<Server>
        {
            new Server("irc.gamesurge.net", "GameSurge", new int[1] { 6667 }),
            new Server("Burstfire.UK.EU.GameSurge.net", "GameSurge London, UK", new int[3] { 6667, 6668, 7000 }),
            new Server("ColoCrossing.IL.US.GameSurge.net", "GameSurge Chicago, IL", new int[5] { 6660, 6666, 6667, 6668, 6669 }),
            new Server("Gameservers.NJ.US.GameSurge.net", "GameSurge Newark, NJ", new int[7] { 6665, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("Krypt.CA.US.GameSurge.net", "GameSurge Santa Ana, CA", new int[4] { 6666, 6667, 6668, 6669 }),
            new Server("NuclearFallout.WA.US.GameSurge.net", "GameSurge Seattle, WA", new int[2] { 6667, 5960 }),
            new Server("Portlane.SE.EU.GameSurge.net", "GameSurge Stockholm, Sweden", new int[5] { 6660, 6666, 6667, 6668, 6669 }),
            new Server("Prothid.NY.US.GameSurge.Net", "GameSurge NYC, NY", new int[7] { 5960, 6660, 6666, 6667, 6668, 6669, 6697 }),
            new Server("TAL.DE.EU.GameSurge.net", "GameSurge Wuppertal, Germany", new int[5] { 6660, 6666, 6667, 6668, 6669 }),
            new Server("208.167.237.120", "GameSurge IP 208.167.237.120", new int[7] {  6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("192.223.27.109", "GameSurge IP 192.223.27.109", new int[7] {  6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("108.174.48.100", "GameSurge IP 108.174.48.100", new int[7] { 6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("208.146.35.105", "GameSurge IP 208.146.35.105", new int[7] { 6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("195.8.250.180", "GameSurge IP 195.8.250.180", new int[7] { 6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("91.217.189.76", "GameSurge IP 91.217.189.76", new int[7] { 6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
            new Server("195.68.206.250", "GameSurge IP 195.68.206.250", new int[7] { 6660, 6666, 6667, 6668, 6669, 7000, 8080 }),
        }.AsReadOnly();

        public static readonly IList<ColorX> Colors = new List<ColorX>
        {
            new ColorX("Ĭ����ɫ", Brushes.White, 0),
            new ColorX("Ĭ����ɫ #2", Brushes.White, 1),
            new ColorX("ǳ��ɫ", Brushes.LightBlue, 2),
            new ColorX("��ɫ", Brushes.ForestGreen, 3),
            new ColorX("���ɫ", Brushes.MediumVioletRed, 4),
            new ColorX("��ɫ", Brushes.Red, 5),
            new ColorX("��ɫ", Brushes.MediumOrchid, 6),
            new ColorX("��ɫ", Brushes.Orange, 7),
            new ColorX("��ɫ", Brushes.Yellow, 8),
            new ColorX("ǳ��ɫ", Brushes.Lime, 9),
            new ColorX("����ɫ", Brushes.Turquoise, 10),
            new ColorX("����ɫ", Brushes.LightSkyBlue, 11),
            new ColorX("��ɫ", Brushes.RoyalBlue, 12),
            new ColorX("�ۺ�ɫ", Brushes.Fuchsia, 13),
            new ColorX("��ɫ", Brushes.LightGray, 14),
            new ColorX("��ɫ #2", Brushes.Gray, 15),
        }.AsReadOnly();

        public event EventHandler<string> ChatLogRep;
        internal void OnChatLogRep(string s)
        {
            if (ChatLogRep != null) ChatLogRep(this, s);
        }

        public void ConnectToServer()
        {
            for (int sid = 0; sid < Servers.Count; sid++)
            {
                var serv = Servers[sid];
                for (int i = 0; i < serv.Ports.Length; i++)
                {
                    try
                    {
                        TcpClient client = new TcpClient(AddressFamily.InterNetwork);
                        var result = client.BeginConnect(serv.Host, serv.Ports[i], null, null);
                        result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3.0), false);
                        OnChatLogRep($"���������� {serv.Host}:{serv.Ports[i]}");
                        if (!client.Connected)
                        {
                            OnChatLogRep($"������ {serv.Host}:{serv.Ports[i]} ��ʱ!");
                        }
                        else if (client.Connected)
                        {
                            OnChatLogRep($"�ɹ������� {serv.Host}:{serv.Ports[i]}");
                            //client.EndConnect(result);

                            Socket = client.Client;

                            ReadBuffer = new byte[ReadBufferLength];
                            ReadBufferIndex = 0;
                            PingTimer = new Timer(30000);
                            PingTimer.Elapsed += (sender, e) =>
                            {
                                if (!string.IsNullOrEmpty(ServerNameFromPing))
                                    SendRawMessage("PING :{0}", ServerNameFromPing);
                            };
                            var checkQueue = new Timer(1000);
                            checkQueue.Elapsed += (sender, e) =>
                            {
                                string nextMessage;
                                if (WriteQueue.Count > 0)
                                {
                                    while (!WriteQueue.TryDequeue(out nextMessage)) ;
                                    SendRawMessage(nextMessage);
                                }
                            };
                            checkQueue.Start();

                            ConnectComplete(result);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnChatLogRep(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Connects to the IRC server.
        /// </summary>
        public void ConnectAsync()
        {
            if (Socket != null && Socket.Connected) throw new InvalidOperationException("Socket is already connected to server.");
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ReadBuffer = new byte[ReadBufferLength];
            ReadBufferIndex = 0;
            PingTimer = new Timer(30000);
            PingTimer.Elapsed += (sender, e) => 
            {
                if (!string.IsNullOrEmpty(ServerNameFromPing))
                    //SendRawMessage("PING :{0}", ServerNameFromPing);
                    SendRawMessage($"PING LAG{new Random().Next(100000, 999999)}");
            };
            var checkQueue = new Timer(1000);
            checkQueue.Elapsed += (sender, e) =>
            {
                string nextMessage;
                if (WriteQueue.Count > 0)
                {
                    while (!WriteQueue.TryDequeue(out nextMessage));
                    SendRawMessage(nextMessage);
                }
            };
            checkQueue.Start();
            Socket.BeginConnect(ServerHostname, ServerPort, ConnectComplete, null);
        }

        /// <summary>
        /// Send a QUIT message and disconnect.
        /// </summary>
        public void Quit()
        {
            Quit(null);
        }

        /// <summary>
        /// Send a QUIT message with a reason and disconnect.
        /// </summary>
        public void Quit(string reason)
        {
            if (reason == null)
                SendRawMessage("QUIT");
            else
                SendRawMessage("QUIT :{0}", reason);
            Socket.BeginDisconnect(false, ar =>
            {
                Socket.EndDisconnect(ar);
                NetworkStream.Dispose();
                NetworkStream = null;
            }, null);
            PingTimer.Dispose();
        }

        private void ConnectComplete(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);
                NetworkStream = new NetworkStream(Socket);
                if (UseSSL)
                {
                    if (IgnoreInvalidSSL)
                        NetworkStream = new SslStream(NetworkStream, false, (sender, certificate, chain, policyErrors) => true);
                    else
                        NetworkStream = new SslStream(NetworkStream);
                    ((SslStream)NetworkStream).AuthenticateAsClient(ServerHostname);
                }

                NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length, DataRecieved, null);
                // Begin capability negotiation
                //SendRawMessage("CAP LS 302");
                // Write login info
                if (!string.IsNullOrEmpty(User.Password))
                    SendRawMessage("PASS {0}", User.Password);
                //SendRawMessage("NICK {0}", User.Nick);
                SendRawMessage($"USER MO.{Program.ID} 0 * :3.3.4 MO CnCNet");
                // hostname, servername are ignored by most IRC servers
                SendRawMessage($"NICK {Program.Name}");
                //SendRawMessage("USER {0} hostname servername :{1}", User.User, User.RealName);
                PingTimer.Start();
            }
            catch (SocketException e)
            {
                OnNetworkError(new SocketErrorEventArgs(e.SocketErrorCode));
            }
            catch (Exception e)
            {
                OnError(new Events.ErrorEventArgs(e));
            }
        }

        private void DataRecieved(IAsyncResult result)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                return;
            }

            int length;
            try
            {
                length = NetworkStream.EndRead(result) + ReadBufferIndex;
            }
            catch (IOException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException != null)
                    OnNetworkError(new SocketErrorEventArgs(socketException.SocketErrorCode));
                else
                    OnError(new Events.ErrorEventArgs(e)); // throw;
                return;
            }

            ReadBufferIndex = 0;
            while (length > 0)
            {
                int messageLength = Array.IndexOf(ReadBuffer, (byte)'\n', 0, length);
                if (messageLength == -1) // Incomplete message
                {
                    ReadBufferIndex = length;
                    break;
                }
                messageLength++;
                var message = Encoding.GetString(ReadBuffer, 0, messageLength - 2); // -2 to remove \r\n
                HandleMessage(message);
                Array.Copy(ReadBuffer, messageLength, ReadBuffer, 0, length - messageLength);
                length -= messageLength;
            }
            try
            {
                NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length - ReadBufferIndex, DataRecieved, null);
            }
            catch (IOException e)
            {
                OnError(new Events.ErrorEventArgs(e));
            }
        }

        private void HandleMessage(string rawMessage)
        {
            OnRawMessageRecieved(new RawMessageEventArgs(rawMessage, false));
            var message = new IrcMessage(rawMessage);
            if (Handlers.ContainsKey(message.Command.ToUpper()))
                Handlers[message.Command.ToUpper()](this, message);
            else
            {
                // TODO: Fire an event or something
            }
        }

        public void SendRawMessage2(string message)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                return;
            }

            var data = Encoding.GetBytes(message);
            if (!IsWriting)
            {
                IsWriting = true;
                try
                {
                    NetworkStream.BeginWrite(data, 0, data.Length, MessageSent, message);
                }
                catch (IOException e)
                {
                    OnError(new Events.ErrorEventArgs(e));
                }
            }
            else
            {
                WriteQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Send a raw IRC message. Behaves like /quote in most IRC clients.
        /// </summary>
        public void SendRawMessage(string message, params object[] format)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                return;
            }

            message = string.Format(message, format);
            var data = Encoding.GetBytes(message + "\r\n");

            if (!IsWriting)
            {
                IsWriting = true;
                try
                {
                    NetworkStream.BeginWrite(data, 0, data.Length, MessageSent, message);
                }
                catch (IOException e)
                {
                    OnError(new Events.ErrorEventArgs(e));
                }
            }
            else
            {
                WriteQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Send a raw IRC message. Behaves like /quote in most IRC clients.
        /// </summary>
        public void SendIrcMessage(IrcMessage message)
        {
            SendRawMessage(message.RawMessage);
        }

        private void MessageSent(IAsyncResult result)
        {
            if (NetworkStream == null)
            {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                IsWriting = false;
                return;
            }

            try
            {
                NetworkStream.EndWrite(result);
            }
            catch (IOException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException != null)
                    OnNetworkError(new SocketErrorEventArgs(socketException.SocketErrorCode));
                else
                    throw;
                return;
            }
            finally
            {
                IsWriting = false;
            }

            OnRawMessageSent(new RawMessageEventArgs((string)result.AsyncState, true));

            string nextMessage;
            if (WriteQueue.Count > 0)
            {
                while (!WriteQueue.TryDequeue(out nextMessage));
                SendRawMessage(nextMessage);
            }
        }

        /// <summary>
        /// IRC Error Replies. rfc1459 6.1.
        /// </summary>
        public event EventHandler<Events.ErrorReplyEventArgs> ErrorReply;
        internal void OnErrorReply(Events.ErrorReplyEventArgs e)
        {
            if (ErrorReply != null) ErrorReply(this, e);
        }
        /// <summary>
        /// Raised for errors.
        /// </summary>
        public event EventHandler<Events.ErrorEventArgs> Error;
        internal void OnError(Events.ErrorEventArgs e)
        {
            if (Error != null) Error(this, e);
        }
        /// <summary>
        /// Raised for socket errors. CnCNET does not automatically reconnect.
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> NetworkError;
        internal void OnNetworkError(SocketErrorEventArgs e)
        {
            if (NetworkError != null) NetworkError(this, e);
        }
        /// <summary>
        /// Occurs when a raw message is sent.
        /// </summary>
        public event EventHandler<RawMessageEventArgs> RawMessageSent;
        internal void OnRawMessageSent(RawMessageEventArgs e)
        {
            if (RawMessageSent != null) RawMessageSent(this, e);
        }
        /// <summary>
        /// Occurs when a raw message recieved.
        /// </summary>
        public event EventHandler<RawMessageEventArgs> RawMessageRecieved;
        internal void OnRawMessageRecieved(RawMessageEventArgs e)
        {
            if (RawMessageRecieved != null) RawMessageRecieved(this, e);
        }
        /// <summary>
        /// Occurs when a notice recieved.
        /// </summary>
        public event EventHandler<IrcNoticeEventArgs> NoticeRecieved;
        internal void OnNoticeRecieved(IrcNoticeEventArgs e)
        {
            if (NoticeRecieved != null) NoticeRecieved(this, e);
        }
        /// <summary>
        /// Occurs when the server has sent us part of the MOTD.
        /// </summary>
        public event EventHandler<ServerMOTDEventArgs> MOTDPartRecieved;
        internal void OnMOTDPartRecieved(ServerMOTDEventArgs e)
        {
            if (MOTDPartRecieved != null) MOTDPartRecieved(this, e);
        }
        /// <summary>
        /// Occurs when the entire server MOTD has been recieved.
        /// </summary>
        public event EventHandler<ServerMOTDEventArgs> MOTDRecieved;
        internal void OnMOTDRecieved(ServerMOTDEventArgs e)
        {
            if (MOTDRecieved != null) MOTDRecieved(this, e);
        }
        /// <summary>
        /// Occurs when a private message recieved. This can be a channel OR a user message.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageRecieved;
        internal void OnPrivateMessageRecieved(PrivateMessageEventArgs e)
        {
            if (PrivateMessageRecieved != null) PrivateMessageRecieved(this, e);
        }
        /// <summary>
        /// Occurs when a message is recieved in an IRC channel.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> ChannelMessageRecieved;
        internal void OnChannelMessageRecieved(PrivateMessageEventArgs e)
        {
            if (ChannelMessageRecieved != null) ChannelMessageRecieved(this, e);
        }
        /// <summary>
        /// Occurs when a message is recieved from a user.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> UserMessageRecieved;
        internal void OnUserMessageRecieved(PrivateMessageEventArgs e)
        {
            if (UserMessageRecieved != null) UserMessageRecieved(this, e);
        }
        /// <summary>
        /// Raised if the nick you've chosen is in use. By default, CnCNET will pick a
        /// random nick to use instead. Set ErronousNickEventArgs.DoNotHandle to prevent this.
        /// </summary>
        public event EventHandler<ErronousNickEventArgs> NickInUse;
        internal void OnNickInUse(ErronousNickEventArgs e)
        {
            if (NickInUse != null) NickInUse(this, e);
        }
        /// <summary>
        /// Occurs when a user or channel mode is changed.
        /// </summary>
        public event EventHandler<ModeChangeEventArgs> ModeChanged;
        internal void OnModeChanged(ModeChangeEventArgs e)
        {
            if (ModeChanged != null) ModeChanged(this, e);
        }
        /// <summary>
        /// Occurs when a user joins a channel.
        /// </summary>
        public event EventHandler<ChannelUserEventArgs> UserJoinedChannel;
        internal void OnUserJoinedChannel(ChannelUserEventArgs e)
        {
            if (UserJoinedChannel != null) UserJoinedChannel(this, e);
        }
        /// <summary>
        /// Occurs when a user parts a channel.
        /// </summary>
        public event EventHandler<ChannelUserEventArgs> UserPartedChannel;
        internal void OnUserPartedChannel(ChannelUserEventArgs e)
        {
            if (UserPartedChannel != null) UserPartedChannel(this, e);
        }
        /// <summary>
        /// Occurs when we have received the list of users present in a channel.
        /// </summary>
        public event EventHandler<ChannelEventArgs> ChannelListRecieved;
        internal void OnChannelListRecieved(ChannelEventArgs e)
        {
            if (ChannelListRecieved != null) ChannelListRecieved(this, e);
        }
        /// <summary>
        /// Occurs when we have received the topic of a channel.
        /// </summary>
        public event EventHandler<ChannelTopicEventArgs> ChannelTopicReceived;
        internal void OnChannelTopicReceived(ChannelTopicEventArgs e)
        {
            if (ChannelTopicReceived != null) ChannelTopicReceived(this, e);
        }
        /// <summary>
        /// Occurs when the IRC connection is established and it is safe to begin interacting with the server.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionComplete;
        internal void OnConnectionComplete(EventArgs e)
        {
            if (ConnectionComplete != null) ConnectionComplete(this, e);
        }
        /// <summary>
        /// Occurs when we receive server info (such as max nick length).
        /// </summary>
        public event EventHandler<SupportsEventArgs> ServerInfoRecieved;
        internal void OnServerInfoRecieved(SupportsEventArgs e)
        {
            if (ServerInfoRecieved != null) ServerInfoRecieved(this, e);
        }
        /// <summary>
        /// Occurs when a user is kicked.
        /// </summary>
        public event EventHandler<KickEventArgs> UserKicked;
        internal void OnUserKicked(KickEventArgs e)
        {
            if (UserKicked != null) UserKicked(this, e);
        }
        /// <summary>
        /// Occurs when a WHOIS response is received.
        /// </summary>
        public event EventHandler<WhoIsReceivedEventArgs> WhoIsReceived;
        internal void OnWhoIsReceived(WhoIsReceivedEventArgs e)
        {
            if (WhoIsReceived != null) WhoIsReceived(this, e);
        }
        /// <summary>
        /// Occurs when a user has changed their nick.
        /// </summary>
        public event EventHandler<NickChangedEventArgs> NickChanged;
        internal void OnNickChanged(NickChangedEventArgs e)
        {
            if (NickChanged != null) NickChanged(this, e);
        }
        /// <summary>
        /// Occurs when a user has quit.
        /// </summary>
        public event EventHandler<UserEventArgs> UserQuit;
        internal void OnUserQuit(UserEventArgs e)
        {
            if (UserQuit != null) UserQuit(this, e);
        }
        /// <summary>
        /// Occurs when a WHO (WHOX protocol) is received.
        /// </summary>
        public event EventHandler<WhoxReceivedEventArgs> WhoxReceived;
        internal void OnWhoxReceived(WhoxReceivedEventArgs e)
        {
            WhoxReceived?.Invoke(this, e);
        }
    }
}