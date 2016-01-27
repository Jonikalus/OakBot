using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace OakBot
{
    public class TwitchChatConnection
    {
        #region Fields

        private BotIrcClient ircClient;
        private TwitchCredentials connectedUser;
        private string joinedChannel;
        private bool isBot;

        #endregion

        #region Constructors

        public TwitchChatConnection(TwitchCredentials connectingUser, bool isBot = true)
        {
            this.connectedUser = connectingUser;
            this.isBot = isBot;

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
            joinedChannel = channel;
            ircClient.WriteLineThrottle("JOIN #" + channel);
        }

        public void SendChatMessage(string message)
        {
            IrcClient.WriteLineThrottle(":" + connectedUser.UserName +
                "!" + connectedUser.UserName + "@" + connectedUser.UserName +
                ".tmi.twitch.tv PRIVMSG #" + joinedChannel + " :" + message);
        }

        internal void Run()
        {
            while (true)
            {
                TwitchChatMessage ircMessage = new TwitchChatMessage(ircClient.ReadLine(), connectedUser);

                // Bot account is the main chat account
                if (isBot)
                {
                    Trace.WriteLine(connectedUser.UserName + ":  " + ircMessage.ReceivedLine);
                    switch (ircMessage.Command)
                    {
                        case "PING": // Received PING
                            ircClient.WriteLine("PONG");
                            break;

                        case "MODE": // Received MODE
                            if (ircMessage.Args[1] == "+o")
                            {
                                // Set throttle for current user as operator
                                if (ircMessage.Args[2] == connectedUser.UserName)
                                {
                                    ircClient.throttle = 350;
                                }
                            }
                            else if (ircMessage.Args[1] == "-o")
                            {
                                // Set throttle for current user as member
                                if (ircMessage.Args[2] == connectedUser.UserName)
                                {
                                    ircClient.throttle = 1550;
                                }
                            }
                            break;

                        // Received a list of joined viewers
                        case "353":
                            string[] viewers = ircMessage.Message.Split(' ');
                            foreach (string username in viewers)
                            {
                                Utils.AddToViewersCol(username);
                            }
                        break;

                        // JOIN Event
                        case "JOIN":
                            Utils.AddToViewersCol(ircMessage.Author);
                        break;

                        // PART Event
                        case "PART":
                            Utils.RemoveFromViewersCol(ircMessage.Author);
                        break;
                        
                        // PRIVMSG (Chat Message Received) Event
                        case "PRIVMSG":
                            // Seeing that JOIN Message is not that fast ...
                            Utils.AddToViewersCol(ircMessage.Author);
                            MainWindow.colChatMessages.Add(ircMessage);
                        break;
                    }
                }
                else
                {
                    Trace.WriteLine(connectedUser.UserName + ":  " + ircMessage.ReceivedLine);
                    switch (ircMessage.Command)
                    {
                        case "PING": // Received PING
                            ircClient.WriteLine("PONG");
                            break;

                        case "MODE": // Received MODE
                            if (ircMessage.Args[1] == "+o")
                            {
                                // Set throttle for current user as operator
                                if (ircMessage.Args[2] == connectedUser.UserName)
                                {
                                    ircClient.throttle = 350;
                                }
                            }
                            else if (ircMessage.Args[1] == "-o")
                            {
                                // Set throttle for current user as member
                                if (ircMessage.Args[2] == connectedUser.UserName)
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

        public TwitchCredentials ConnectedUser
        {
            get
            {
                return connectedUser;
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
