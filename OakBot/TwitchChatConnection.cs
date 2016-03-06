using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
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
            // http://tmi.twitch.tv/servers?channel=riotgames
            ircClient = new BotIrcClient("irc.twitch.tv", 6667, connectingUser);

            // Request JOIN/PART notifications for bot account
            if (isBot)
            {
                ircClient.WriteLineThrottle("CAP REQ :twitch.tv/membership");
                ircClient.WriteLineThrottle("CAP REQ :twitch.tv/tags");
                ircClient.WriteLineThrottle("CAP REQ :twitch.tv/commands");
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
                //IrcMessage ircMessage = new IrcMessage(ircClient.ReadLine(), connectedUser);
                string rm = ircClient.ReadLine();
                IrcMessage ircMessage = new IrcMessage(rm, connectedUser);
                Trace.WriteLine(rm);

                // Bot account is the main chat account
                if (isBot)
                {
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
                                MainWindow.colGroups.First(x => x.GroupName == "Guest").Add(new Viewer(username));
                            }
                        break;

                        // JOIN Event
                        case "JOIN":
                            Utils.AddToViewersCol(ircMessage.Author);
                            MainWindow.colGroups.First(x => x.GroupName == "Guest").Add(new Viewer(ircMessage.Author));
                            break;

                        // PART Event
                        case "PART":
                            Utils.RemoveFromViewersCol(ircMessage.Author);
                        break;
                        
                        // PRIVMSG (Chat Message Received) Event
                        case "PRIVMSG":
                            // Seeing that JOIN Message is not that fast ...
                            Utils.AddToViewersCol(ircMessage.Author);
                            MainWindow.colGroups.First(x => x.GroupName == "Guest").Add(new Viewer(ircMessage.Author));

                            // Add the message to the collection
                            MainWindow.colChatMessages.Add(ircMessage);

                            //MainWindow.instance.Dispatcher.BeginInvoke(new Action(delegate
                            //{
                            //    MainWindow.colChatMessages.Add(ircMessage);
                            //}));

                            //App.Current.Dispatcher.BeginInvoke(new Action(delegate
                            //{
                            //    MainWindow.colChatMessages.Add(ircMessage);
                            //}));


                            // Execute command checking and executing in a task to offload receiving
                            new Task(() => {

                                string firstWord = Regex.Match(ircMessage.Message, @"^\S+\b").Value.ToLower();
                                UserCommand foundCommand = MainWindow.colBotCommands.FirstOrDefault(x => 
                                    x.Command == firstWord);

                                if (foundCommand != null)
                                {
                                    foundCommand.ExecuteCommand(ircMessage);
                                }
                                else
                                {
                                    BotCommands.RunBotCommand(firstWord, ircMessage);
                                }

                            }).Start();
                        break;
                    }
                }
                else
                {
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
