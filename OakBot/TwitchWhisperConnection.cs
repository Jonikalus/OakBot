namespace OakBot
{
    public class TwitchWhisperConnection
    {
        #region Private Fields

        private TwitchCredentials _connectedUser;
        private MainWindow _mW;

        //MainWindow.delegateMessage delegateMessage;
        private BotIrcClient ircClient;

        #endregion Private Fields

        #region Public Constructors

        public TwitchWhisperConnection(TwitchCredentials connectingUser, MainWindow window)
        {
            _mW = window;
            //delegateMessage = new MainWindow.delegateMessage(_mW.ResolveDispatchToUI);
            _connectedUser = connectingUser;

            // Connect and login into the whisper server
            ircClient = new BotIrcClient("199.9.253.119", 6667, connectingUser);

            // Request whispers
            ircClient.WriteLineThrottle("CAP REQ :twitch.tv/commands");
        }

        #endregion Public Constructors

        #region Public Properties

        public TwitchCredentials connectedUser
        {
            get
            {
                return _connectedUser;
            }
        }

        //internal void dispatchMessage(IrcMessage message)
        //{
        //    _mW.Dispatcher.BeginInvoke(delegateMessage, message);
        //}
        public BotIrcClient IrcClient
        {
            get
            {
                return ircClient;
            }
        }

        #endregion Public Properties

        #region Internal Methods

        internal void Run()
        {
            while (true)
            {
                IrcMessage ircMessage = new IrcMessage(ircClient.ReadLine(), _connectedUser);

                switch (ircMessage.Command)
                {
                    case "PING": // Received PING
                        ircClient.WriteLine("PONG");
                        break;

                    case "WHISPER": // Chat message
                        //dispatchMessage(ircMessage);
                        break;
                }
            }
        }

        internal void SendWhisper(string whisperMessage)
        {
            ircClient.WriteLineThrottle("PRIVMSG #jtv :" + whisperMessage);
        }

        #endregion Internal Methods
    }
}