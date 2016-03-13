using System;

namespace OakBot.Args
{
    public class ChatMessageReceivedEventArgs : EventArgs
    {
        #region Private Fields

        private IrcMessage message;

        #endregion Private Fields

        #region Public Constructors

        public ChatMessageReceivedEventArgs(IrcMessage message)
        {
            this.message = message;
        }

        #endregion Public Constructors

        #region Public Properties

        public IrcMessage Message
        {
            get
            {
                return message;
            }
        }

        #endregion Public Properties
    }
}