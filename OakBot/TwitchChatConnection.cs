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
        private bool _isStreamer;

        #endregion

        #region Constructors

        public TwitchChatConnection(TwitchCredentials connectingUser, MainWindow window, bool isStreamer = true)
        {
            _window = window;
            _connectedUser = connectingUser;
            _isStreamer = isStreamer;

            // Connect to IRC Twitch and login, then request PART/JOIN messages
            ircClient = new BotIrcClient("irc.twitch.tv", 6667, connectingUser);

            if (isStreamer)
            {
                // Request JOIN/PART notifications
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
                TwitchChatMessage ircMessage = new TwitchChatMessage(ircClient.ReadLine(), _connectedUser);

                switch (ircMessage.command)
                {
                    case "PING": // Received PING
                        ircClient.WriteLine("PONG");
                        break;

                    case "MODE": // Received MODE
                        if (ircMessage.args[1] == "+o")
                        {
                            // Set throttle for current user as operator
                            if (ircMessage.args[2] == _connectedUser.username)
                            {
                                ircClient.throttle = 350;
                            }
                        }
                        else if (ircMessage.args[1] == "-o")
                        {
                            // Set throttle for current user as member
                            if (ircMessage.args[2] == _connectedUser.username)
                            {
                                ircClient.throttle = 1550;
                            }
                        }

                        if (_isStreamer)
                        {
                            new DispatchUI(_window, ircMessage);
                        }

                        break;

                    default: // Send rest to UI
                        if (_isStreamer)
                        {
                            new DispatchUI(_window, ircMessage);
                        }
                        break;
                }
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
