using System.Linq;
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

                // Streamer account only reply on pings and set throttle
                if (_isBot == false)
                {
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

                        default:
                            Trace.WriteLine(_connectedUser.username + ":  " + ircMessage.receivedLine);
                            break;
                    }
                }
                else
                {
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

                        //case "353": // Received list of joined names
                        //    string[] names = chatMessage.message.Split(' ');
                        //    foreach (string name in names)
                        //    {
                        //        var viewerExist = viewerDatabase.Where(
                        //            TwitchUser => TwitchUser.username == chatMessage.author);
                        //        if (viewerExist.Any()) // Viewer exists
                        //        {
                        //            foreach (TwitchUser viewer in viewerExist)
                        //            {
                        //                colViewers.Add(viewer);
                        //            }
                        //        }
                        //        else // new viewer
                        //        {
                        //            TwitchUser newViewer = new TwitchUser(chatMessage.author);
                        //            viewerDatabase.Add(newViewer);
                        //            colViewers.Add(newViewer);
                        //        }
                        //    }
                        //    break;

                        case "JOIN": // Person joined channel
                            var viewerJoin = MainWindow.colViewers.Where(TwitchUser =>
                                TwitchUser.username == ircMessage.author);

                            if (!viewerJoin.Any())
                            {
                                var viewerExists = MainWindow.viewerDatabase.Where(TwitchUser =>
                                    TwitchUser.username == ircMessage.author);

                                // If viewer exists add a refference to that
                                // in the colViewers.
                                if (viewerExists.Any())
                                {
                                    foreach (TwitchUser viewer in viewerExists)
                                    {
                                        MainWindow.colViewers.Add(viewer);
                                    }
                                }
                                // If viewer does not exists create new Viewer
                                // and add to that refference to colViewers.
                                else
                                {
                                    TwitchUser newViewer = new TwitchUser(ircMessage.author);
                                    MainWindow.viewerDatabase.Add(newViewer);
                                    MainWindow.colViewers.Add(newViewer);
                                }
                            }

                            break;

                        case "PART": // Person left channel
                            try
                            {
                                var viewerPart = MainWindow.colViewers.Where(TwitchUser =>
                                TwitchUser.username == ircMessage.author);

                                foreach (TwitchUser viewer in viewerPart)
                                {
                                    MainWindow.colViewers.Remove(viewer);
                                }
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                break;
                            }

                        case "PRIVMSG":
                            MainWindow.colChatMessages.Add(ircMessage);
                            break;

                        case "WHISPER":
                            MainWindow.colChatMessages.Add(ircMessage);
                            break;

                        default:
                            Trace.WriteLine(_connectedUser.username + ":  " + ircMessage.receivedLine);
                            break;
                    }
                }
            }
        }

        //internal void dispatchMessage(TwitchChatMessage message)
        //{
        //
        //    _mW.Dispatcher.BeginInvoke(delegateMessage, message);
        //}

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
