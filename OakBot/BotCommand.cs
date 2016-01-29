using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    public class BotCommand
    {
        #region Fields

        private string command;
        private string response;
        private int gCooldownSec;

        private DateTime lastUsed;

        #endregion

        #region Constructor

        public BotCommand(string command, string response, int gCooldownSec)
        {
            this.command = command.ToLower();
            this.response = response;
            this.gCooldownSec = gCooldownSec;
        }

        #endregion

        #region Methods

        public void ExecuteCommand(TwitchCredentials account, string user)
        {
            // Check if not on global cooldown
            // using Substract on DateTime creates TimeSpan which as TotalSeconds
            if (DateTime.Now.Subtract(lastUsed).TotalSeconds > gCooldownSec)
            {
                // Parse response here with fancy doodly-does $user $target $time eg

                // Send the message to IRC and set the new lastUsed timestamp
                MainWindow.instance.botChatConnection.SendChatMessage(response);
                lastUsed = DateTime.Now;

                // Create a message object to display in the chat
                MainWindow.colChatMessages.Add(new TwitchChatMessage(account.UserName, response));
            }
        }

        #endregion

        #region Fields

        public string Command
        {
            get
            {
                return command;
            }
        }

        #endregion

    }
}
