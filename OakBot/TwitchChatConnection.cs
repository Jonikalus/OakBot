using System;
using System.Windows.Media;
using System.Diagnostics;

namespace OakBot
{
    class TwitchChatConnection
    {
        #region Fields

        private MainWindow _window;
        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;
        private bool _isBot;
        private TwitchChatChannel _joinedChannel;

        #endregion

        #region Constructors

        public TwitchChatConnection(TwitchCredentials connectingUser, MainWindow window, bool isBot = false)
        {
            _window = window;
            _connectedUser = connectingUser;
            _isBot = isBot;

            // Connect to IRC Twitch and login, then request PART/JOIN messages
            ircClient = new BotIrcClient("irc.twitch.tv", 6667, connectingUser);

            if (!isBot) // Request JOIN/PART notifications on the main account.
            {
                ircClient.WriteLineThrottle("CAP REQ :twitch.tv/membership");
            }   
        }

        #endregion

        #region Methods

        internal void JoinChannel(TwitchChatChannel channel)
        {
            _joinedChannel = channel;
            ircClient.WriteLineThrottle("JOIN #" + channel.name);
        }

        internal void Run()
        {            
            while (true)
            {
                TwitchChatEvent cEvent = new TwitchChatEvent(ircClient.ReadLine());
                EventHandler(cEvent);
            }
        }

        internal void EventHandler(TwitchChatEvent chatEvent)
        {
            //DispatchUI toUI;

            switch (chatEvent.command)
            {
                case "PING": // Received PING
                    ircClient.WriteLine("PONG");
                    break;

                case "MODE": // Received MODE
                    if (chatEvent.args[1] == "+o")
                    {
                        // Set throttle for current user as operator
                        if (chatEvent.args[2] == _connectedUser.username)
                        {
                            ircClient.throttle = 350;
                        }
                    }
                    else if (chatEvent.args[1] == "-o")
                    {
                        // Set throttle for current user as member
                        if (chatEvent.args[2] == _connectedUser.username)
                        {
                            ircClient.throttle = 1550;                    
                        }
                    }

                    if (!_isBot)
                    {
                        new DispatchUI(_window, chatEvent);
                    }

                    break;

                case "JOIN": // Person Joined the channel
                    if (!_isBot)
                    {
                        new DispatchUI(_window, chatEvent);
                    }
                    break;

                case "PART": // Person left the channel
                    if (!_isBot)
                    {
                        new DispatchUI(_window, chatEvent);
                    }
                    break;

                case "PRIVMSG": // Chat message
                    if (!_isBot)
                    {
                        new DispatchUI(_window, chatEvent);
                    }
                    break;

                default: // Unknown event
                    new DispatchUI(_window, chatEvent);
                    break;
            }
        }

        public void SendMessage(string message)
        {
            _joinedChannel.SendChatMessage(message);
        }

        #endregion

        #region Properies

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
