using System;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace OakBot
{
    public enum uType
    {
        BOT,
        VIEWER,
        MODERATOR,
        GLOBALMODERATOR,
        ADMIN,
        STAFF
    }

    public class IrcMessage
    {
        #region Fields

        // Tracking
        private DateTime timestamp;
        private TwitchCredentials messageSource;

        // Basic IRC Message 
        private string author;
        private string host;
        private string command;
        private string[] arguments;
        private string message;

        // IRC v3 TAGS
        private string nameColor;
        private string displayName;
        private string emotes;
        private bool moderator;
        //private string roomId;
        private bool subscriber;
        private bool turbo;
        private int userId;
        private uType userType;


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to be used to parse receoved IRC message.
        /// </summary>
        /// <param name="receivedLine">Received IRC message.</param>
        /// <param name="loggedinAccount">Account that is logged in to the IRC.</param>
        public IrcMessage(string receivedLine, TwitchCredentials loggedinAccount)
        {
            this.timestamp = DateTime.Now;
            this.messageSource = loggedinAccount;

            // First get all arguments if starts with @
            if (receivedLine.StartsWith("@"))
            {
                MatchCollection ircTags = Regex.Matches(receivedLine, @"(?<arg>[\w-]+)=(?<value>[\w:#,-]*);?");
                foreach (Match m in ircTags)
                {
                    switch (m.Groups["arg"].Value)
                    {
                        case "color":
                            nameColor = m.Groups["value"].Value;
                            break;

                        case "display-name":
                            displayName = m.Groups["value"].Value;
                            break;

                        case "emotes":
                            emotes = m.Groups["value"].Value;
                            break;

                        case "mod":
                            moderator = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "subscriber":
                            subscriber = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "turbo":
                            turbo = (!string.IsNullOrEmpty(m.Groups["value"].Value) && m.Groups["value"].Value == "1");
                            break;

                        case "user-id":
                            userId = int.Parse(m.Groups["value"].Value);
                            break;

                        case "user-type":
                            switch (m.Groups["value"].Value)
                            {
                                case "mod":
                                    userType = uType.MODERATOR;
                                    break;

                                case "global_mod":
                                    userType = uType.GLOBALMODERATOR;
                                    break;

                                case "admin":
                                    userType = uType.ADMIN;
                                    break;

                                case "staff":
                                    userType = uType.STAFF;
                                    break;

                                default:
                                    userType = uType.VIEWER;
                                    break;
                            }
                            break;

                    }
                }
            }

            // Get the base IRC message
            //Match ircMessage = Regex.Match(receivedLine, @"(?<!\S)(?::(?:(?<author>\w+)!)?(?<host>\S+) )?(?<command>\w+)(?: (?!:)(?<args>.+?))?(?: :(?<message>.+))?$");
            Match ircMessage = Regex.Match(receivedLine,
                @"(?<!\S)(?::(?:(?<author>\w+)!)?(?<host>\S+) )?(?<command>\w+)(?: (?<args>.+?))?(?: :(?<message>.+))?$");

            author = ircMessage.Groups["author"].Value;
            host = ircMessage.Groups["host"].Value;
            command = ircMessage.Groups["command"].Value;
            arguments = ircMessage.Groups["args"].Value.Split(' ');
            message = ircMessage.Groups["message"].Value;
            
        }

        /// <summary>
        /// Constructor to use to create manually an chat message.
        /// </summary>
        /// <param name="author">Author of the message.</param>
        /// <param name="message">Message content.</param>
        public IrcMessage(string author, string message)
        {
            this.timestamp = DateTime.Now;
            this.message = message;
            this.author = author;
            this.userType = uType.BOT;
        }

        #endregion

        #region Properties

        public string ShortTime
        {
            get
            {
                return timestamp.ToShortTimeString();
            }
        }

        public string Command
        {
            get
            {
                return command;
            }
        }

        public string[] Args
        {
            get
            {
                return arguments;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        public string NameColor
        {
            get
            {
                return string.IsNullOrEmpty(nameColor) ? "#000000" : nameColor;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(displayName) ? author : displayName;
            }
        }

        public uType UserType
        {
            get
            {
                return userType;
            }
        }

        #endregion
    }
}