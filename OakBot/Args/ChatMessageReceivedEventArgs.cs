using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OakBot;

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
