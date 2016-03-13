using System;

namespace OakBot.Helpers
{
    public class TwitchException : Exception
    {
        #region Public Constructors

        public TwitchException()
        {
        }

        public TwitchException(string message) : this(message, null)
        {
        }

        public TwitchException(string message, Exception inner) : base(message, inner)
        {
        }

        #endregion Public Constructors
    }
}