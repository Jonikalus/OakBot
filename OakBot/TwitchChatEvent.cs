using System.Text.RegularExpressions;

namespace OakBot
{
    public class TwitchChatEvent
    {
        #region Fields

        private string _received;
        private string _command;
        private string[] _arguments;
        private string _author;
        private string _message;

        #endregion

        #region Constructors

        public TwitchChatEvent(string receivedLine)
        {
            if (string.IsNullOrEmpty(receivedLine))
            {
                _command = "NULL";
            }
            else
            {
                Match parsedLine = Regex.Match(receivedLine,
                    @"^(?:[:](?:(?<author>\S+)[!])?\S+ )?(?<command>\S+)(?: (?!:)(?<arguments>.+?))?(?: [:](?<message>.+))?$");

                _received = receivedLine;
                _command = parsedLine.Groups["command"].Value.Trim();
                _arguments = parsedLine.Groups["arguments"].Value.Trim().Split(' ');
                _author = parsedLine.Groups["author"].Value.Trim();
                _message = parsedLine.Groups["message"].Value.Trim();
            }
        }

        #endregion

        #region Properties

        public string line
        {
            get
            {
                return _received;
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