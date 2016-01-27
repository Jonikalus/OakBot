using System;
using System.Text.RegularExpressions;

namespace OakBot
{
    public class TwitchChatMessage
    {
        #region Fields

        private DateTime _timestamp;
        private TwitchCredentials _messageSource;
        private string _receivedLine;
        private string _command;
        private string[] _arguments;
        private string _author;
        private string _message;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to be used to parse receoved IRC message.
        /// </summary>
        /// <param name="receivedLine">Received IRC message.</param>
        /// <param name="loggedinAccount">Account that is logged in to the IRC.</param>
        public TwitchChatMessage(string receivedLine, TwitchCredentials loggedinAccount)
        {
            _timestamp = DateTime.Now;
            _messageSource = loggedinAccount;
            _receivedLine = receivedLine;

            try
            {
                Match parsedLine = Regex.Match(receivedLine,
                @"^(?:[:](?:(?<author>\S+)[!])?\S+ )?(?<command>\S+)(?: (?!:)(?<arguments>.+?))?(?: [:](?<message>.+))?$");

                _command = parsedLine.Groups["command"].Value.Trim();
                _arguments = parsedLine.Groups["arguments"].Value.Trim().Split(' ');
                _author = parsedLine.Groups["author"].Value.Trim();
                _message = parsedLine.Groups["message"].Value.Trim();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Constructor to use to create manually an chat message.
        /// </summary>
        /// <param name="author">Author of the message.</param>
        /// <param name="message">Message content.</param>
        public TwitchChatMessage(string author, string message)
        {
            _timestamp = DateTime.Now;
            _message = message;
            _author = author;
        }

        #endregion

        #region Properties

        public string ShortTime
        {
            get
            {
                return _timestamp.ToShortTimeString();
            }
        }

        public string ReceivedLine
        {
            get
            {
                return _receivedLine;
            }
        }

        public string Command
        {
            get
            {
                return _command;
            }
        }

        public string[] Args
        {
            get
            {
                return _arguments;
            }
        }

        public string Author
        {
            get
            {
                return _author;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        #endregion
    }
}