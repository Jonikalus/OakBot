using System;
using System.Windows.Media;
using System.Diagnostics;

namespace OakBot
{
    public class TwitchChatConnection
    {
        #region Fields

        private MainWindow _mW;
        MainWindow.delegateMessage delegateMessage;
        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;
        private string _joinedChannel;
        private bool _isStreamer;

        #endregion

        #region Constructors

        public TwitchChatConnection(TwitchCredentials connectingUser, MainWindow window, bool isStreamer = true)
        {
            _mW = window;
            delegateMessage = new MainWindow.delegateMessage(_mW.ResolveDispatchToUI);
            _connectedUser = connectingUser;
            _isStreamer = isStreamer;

            // Connect to IRC Twitch and login with given TwitchCredentials
            ircClient = new BotIrcClient("irc.twitch.tv", 6667, connectingUser);
            
            // Request JOIN/PART notifications
            ircClient.WriteLineThrottle("CAP REQ :twitch.tv/membership");  
        }

        #endregion

        #region Methods

        public void JoinChannel(string channel)
        {
            _joinedChannel = channel;
            ircClient.WriteLineThrottle("JOIN #" + channel);
        }

        public void SendChatMessage(string message)
        {
            IrcClient.WriteLineThrottle(":" + _connectedUser.username +
                "!" + _connectedUser.username + "@" + _connectedUser.username +
                ".tmi.twitch.tv PRIVMSG #" + _joinedChannel + " :" + message);
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
                        break;

                    default: // Chat message
                        if (_isStreamer)
                        {
                            dispatchMessage(ircMessage);
                        }
                        break;
                }
            }
        }

        internal void dispatchMessage(TwitchChatMessage message)
        {

            _mW.Dispatcher.BeginInvoke(delegateMessage, message);
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
