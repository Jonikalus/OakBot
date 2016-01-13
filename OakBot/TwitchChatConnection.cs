using System;
using System.Windows.Media;
using System.Diagnostics;

namespace OakBot
{
    public class TwitchChatConnection
    {
        #region Fields

        private MainWindow _window;
        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;
        private TwitchUser _joinedChannel;
        private bool _isBot;

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

        public void JoinChannel(TwitchUser channel)
        {
            _joinedChannel = channel;
            ircClient.WriteLineThrottle("JOIN #" + channel.username);
        }

        public void SendChatMessage(string message)
        {
            IrcClient.WriteLineThrottle(":" + connectedUser.username +
                "!" + connectedUser.username + "@" + connectedUser.username +
                ".tmi.twitch.tv PRIVMSG #" + _joinedChannel.username + " :" + message);
        }

        internal void Run()
        {            
            while (true)
            {
                TwitchChatMessage message = new TwitchChatMessage(ircClient.ReadLine(), _connectedUser);
                EventHandler(message);
            }
        }

        internal void EventHandler(TwitchChatMessage cMessage)
        {
            //DispatchUI toUI;

            switch (cMessage.command)
            {
                case "PING": // Received PING
                    ircClient.WriteLine("PONG");
                    break;

                case "MODE": // Received MODE
                    if (cMessage.args[1] == "+o")
                    {
                        // Set throttle for current user as operator
                        if (cMessage.args[2] == _connectedUser.username)
                        {
                            ircClient.throttle = 350;
                        }
                    }
                    else if (cMessage.args[1] == "-o")
                    {
                        // Set throttle for current user as member
                        if (cMessage.args[2] == _connectedUser.username)
                        {
                            ircClient.throttle = 1550;                    
                        }
                    }

                    if (!_isBot)
                    {
                        new DispatchUI(_window, cMessage);
                    }

                    break;

                case "JOIN": // Person Joined the channel
                    if (!_isBot)
                    {
                        new DispatchUI(_window, cMessage);
                    }
                    break;

                case "PART": // Person left the channel
                    if (!_isBot)
                    {
                        new DispatchUI(_window, cMessage);
                    }
                    break;

                case "PRIVMSG": // Chat message
                    if (!_isBot)
                    {
                        new DispatchUI(_window, cMessage);
                    }
                    break;

                default: // Unknown event
                    new DispatchUI(_window, cMessage);
                    break;
            }
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
