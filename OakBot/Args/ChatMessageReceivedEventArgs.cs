using System;

namespace OakBot.Args
{
    public class ChatMessageReceivedEventArgs : EventArgs
    {
        private IrcMessage message;

        public IrcMessage Message
        {
            get
            {
                return message;
            }
        }

        public ChatMessageReceivedEventArgs(IrcMessage message)
        {
            this.message = message;
        }
    }
}