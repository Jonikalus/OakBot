using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace OakBot
{
    public class TwitchChatConnection
    {
        #region Fields

        private BotIrcClient ircClient;
        private TwitchCredentials _connectedUser;
        private string _joinedChannel;
        private bool _isBot;

        #endregion

        #region Constructors

        public TwitchChatConnection(TwitchCredentials connectingUser, bool isBot = true)
        {
            _connectedUser = connectingUser;
            _isBot = isBot;

            // Connect to IRC Twitch and login with given TwitchCredentials
            ircClient = new BotIrcClient("irc.twitch.tv", 6667, connectingUser);

            // Request JOIN/PART notifications for bot account
            if (isBot)
            {
                ircClient.WriteLineThrottle("CAP REQ :twitch.tv/membership");
            }   
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

                // Bot account is the main chat account
                if (_isBot)
                {
                    Trace.WriteLine(_connectedUser.username + ":  " + ircMessage.receivedLine);
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

                        // Received a list of joined viewers
                        case "353":
                            string[] viewers = ircMessage.message.Split(' ');
                            foreach (string username in viewers)
                            {
                                Utils.AddToViewersCol(username);
                            }
                        break;

                        // JOIN Event
                        case "JOIN":
                            Utils.AddToViewersCol(ircMessage.author);
                        break;

                        // PART Event
                        case "PART":
                            Utils.RemoveFromViewersCol(ircMessage.author);
                        break;
                        
                        // PRIVMSG (Chat Message Received) Event
                        case "PRIVMSG":
                            // Seeing that JOIN Message is not that fast ...
                            Utils.AddToViewersCol(ircMessage.author);
                            MainWindow.colChatMessages.Add(ircMessage);
                        break;
                    }
                }
                else
                {
                    Trace.WriteLine(_connectedUser.username + ":  " + ircMessage.receivedLine);
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
                    }
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
