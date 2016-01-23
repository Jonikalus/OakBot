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
                Trace.WriteLine(_connectedUser.username + ":  " + ircMessage.receivedLine);

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

                        // Received a list of joined viewers
                        case "353":
                            string[] viewers = ircMessage.message.Split(' ');
                            foreach (string name in viewers)
                            {
                                // First check if viewer is not already in the viewers list
                                var isInViewList1 = MainWindow.colViewers.FirstOrDefault(x => x.username == name);
                                if (isInViewList1 == null)
                                {
                                    // Check if viewer exists in database to refer to
                                    var isInDatabase = MainWindow.viewerDatabase.FirstOrDefault(x => x.username == name);
                                    if (isInDatabase != null)
                                    { // is in database
                                        MainWindow.colViewers.Add(isInDatabase);
                                    }
                                    else
                                    { // is not in database
                                        TwitchUser newViewer = new TwitchUser(name);
                                        MainWindow.viewerDatabase.Add(newViewer);
                                        MainWindow.colViewers.Add(newViewer);
                                    }
                                }
                            }
                        break;

                        // JOIN Event
                        case "JOIN":
                            // First check if JOINed viewer is not already in the viewers list
                            var isInViewList2 = MainWindow.colViewers.FirstOrDefault(x => x.username == ircMessage.author);
                            if(isInViewList2 == null)
                            {
                                // Check if viewer exists in database to refer to
                                var isInDatabase = MainWindow.viewerDatabase.FirstOrDefault(x => x.username == ircMessage.author);
                                if(isInDatabase != null)
                                { // is in database
                                    MainWindow.colViewers.Add(isInDatabase);
                                }
                                else
                                { // is not in database
                                    TwitchUser newViewer = new TwitchUser(ircMessage.author);
                                    MainWindow.viewerDatabase.Add(newViewer);
                                    MainWindow.colViewers.Add(newViewer);
                                }
                            }
                        break;

                        // PART Event
                        case "PART":
                            // Check if PARTing viewer is in the viewers list
                            var toRemove = MainWindow.colViewers.FirstOrDefault(x => x.username == ircMessage.author);
                            if(toRemove != null)
                            {
                                MainWindow.colViewers.Remove(toRemove);
                            }

                            // other method (itteration)
                            //MainWindow.colViewers.Where(x => x.username == ircMessage.author).ToList().ForEach(
                            //    e => MainWindow.colViewers.Remove(e));
                        break;
                        
                        // PRIVMSG (Chat Message Received) Event
                        case "PRIVMSG":
                            MainWindow.colChatMessages.Add(ircMessage);
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
