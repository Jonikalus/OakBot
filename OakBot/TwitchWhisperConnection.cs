using System;

namespace OakBot
{
    class TwitchWhisperConnection
    {
        #region Fields

        private MainWindow _window;
        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;

        #endregion

        #region Constructors

        public TwitchWhisperConnection(TwitchCredentials connectingUser, MainWindow window)
        {
            _window = window;
            _connectedUser = connectingUser;

            // Connect and login into the whisper server
            ircClient = new BotIrcClient("199.9.253.119", 6667, connectingUser);

            // Request whispers
            ircClient.WriteLineThrottle("CAP REQ :twitch.tv/commands");
        }

        #endregion

        #region Methods

        internal void Run()
        {
            while (true)
            {
                TwitchChatEvent cEvent = new TwitchChatEvent(ircClient.ReadLine());
                EventHandler(cEvent);
            }
        }

        internal void SendWhisper(string whisperMessage)
        {
            ircClient.WriteLineThrottle("PRIVMSG #jtv :" + whisperMessage);
        }

        internal void EventHandler(TwitchChatEvent chatEvent)
        {
            switch (chatEvent.command)
            {
                case "PING": // Received PING
                    ircClient.WriteLine("PONG");
                    break;

                case "WHISPER": // Chat message
                    new DispatchUI(_window, chatEvent);
                    break;
 
                default: // Unknown event
                    new DispatchUI(_window, chatEvent);
                    break;
            }
        }

        #endregion

        #region Properties

        public TwitchCredentials connectedUser
        {
            get
            {
                return _connectedUser;
            }
        }

        public BotIrcClient IrcClient
        {
            get
            {
                return ircClient;
            }
        }

        #endregion
    }
}