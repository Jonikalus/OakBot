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

        public TwitchChatMessage(string receivedLine, TwitchCredentials loggedinAccount)
        {
            Match parsedLine = Regex.Match(receivedLine,
                @"^(?:[:](?:(?<author>\S+)[!])?\S+ )?(?<command>\S+)(?: (?!:)(?<arguments>.+?))?(?: [:](?<message>.+))?$");

            _timestamp = DateTime.Now;
            _messageSource = loggedinAccount;
            _receivedLine = receivedLine;
            _command = parsedLine.Groups["command"].Value.Trim();
            _arguments = parsedLine.Groups["arguments"].Value.Trim().Split(' ');
            _author = parsedLine.Groups["author"].Value.Trim();
            _message = parsedLine.Groups["message"].Value.Trim();
        }

        #endregion

        #region Properties

        public DateTime timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public string shortTime
        {
            get
            {
                return _timestamp.ToShortTimeString();
            }
        }

        public string receivedLine
        {
            get
            {
                return _receivedLine;
            }
        }

        public string command
        {
            get
            {
                return _command;
            }
        }

        public string[] args
        {
            get
            {
                return _arguments;
            }
        }

        public string author
        {
            get
            {
                return _author;
            }
        }

        public string message
        {
            get
            {
                return _message;
            }
        }

        #endregion
    }
}