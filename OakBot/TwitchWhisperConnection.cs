using System;

namespace OakBot
{
    public class TwitchWhisperConnection
    {
        #region Fields

        private MainWindow _mW;
        //MainWindow.delegateMessage delegateMessage;
        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;

        #endregion

        #region Constructors

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

        #endregion

        #region Methods

        internal void SendWhisper(string whisperMessage)
        {
            ircClient.WriteLineThrottle("PRIVMSG #jtv :" + whisperMessage);
        }

        internal void Run()
        {
            while (true)
            {
                TwitchChatMessage ircMessage = new TwitchChatMessage(ircClient.ReadLine(), _connectedUser);

                switch (ircMessage.command)
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

        //internal void dispatchMessage(TwitchChatMessage message)
        //{
        //    _mW.Dispatcher.BeginInvoke(delegateMessage, message);
        //}

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