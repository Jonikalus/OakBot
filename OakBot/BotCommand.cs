using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    public enum rank
    {
        viewer,
        regular,
        vip1,
        vip2,
        vip3,
        mod,
        bot
    }

    public class BotCommand
    {
        #region Fields

        private bool sendAsStreamer;
        private string command;
        private string response;

        private int gCooldownSec;
        private int uCooldownSec;

        private int costViewer;
        private int costRegular;
        private int costVIP1;
        private int costVIP2;
        private int costVIP3;
        private int costMod;

        private rank rankRequired;

        private DateTime lastUsed;
        private Dictionary<string, DateTime> viewerLastUsed = new Dictionary<string, DateTime>();

        #endregion

        #region Constructor

        public BotCommand(string command, string response, int gCooldownSec,
            int uCooldownSec, bool sendAsStreamer = false)
        {
            this.command = command.ToLower();
            this.response = response;
            this.gCooldownSec = gCooldownSec;
            this.uCooldownSec = uCooldownSec;
            this.sendAsStreamer = sendAsStreamer;

            this.costViewer = 0;
            this.costRegular = 0;
            this.costVIP1 = 0;
            this.costVIP2 = 0;
            this.costVIP3 = 0;
            this.costMod = 0;
        }

        #endregion

        #region Methods

        public void ExecuteCommand(string receivedLine, string user)
        {
            // Check if not on global cooldown
            // using Substract on DateTime creates TimeSpan which as TotalSeconds
            if (gCooldownSec == 0 || DateTime.Now.Subtract(lastUsed).TotalSeconds > gCooldownSec)
            {
                // Check if user is not on cooldown
                DateTime viewerLastUse;
                viewerLastUsed.TryGetValue(user, out viewerLastUse);
                if (uCooldownSec == 0 ||                                                // If cooldown is 0
                    viewerLastUse == null ||                                            // If "user" is not in CD list
                    DateTime.Now.Subtract(viewerLastUse).TotalSeconds > uCooldownSec)   // If "user" is not on cooldown
                {

                    // Get viewer's TwitchViewer object
                    TwitchViewer viewer = MainWindow.colDatabase.FirstOrDefault(x => x.UserName == user);

                    // Check rank
                    if(true)
                    {
                        // Parse response here with fancy doodly-does $user $target $time eg


                        // Set timestamps
                        lastUsed = DateTime.Now;
                        if (viewerLastUse == null)
                        {
                            viewerLastUsed.Add(user, DateTime.Now);
                        }
                        else
                        {
                            viewerLastUsed[user] = DateTime.Now;
                        }


                        // Send the response
                        if (sendAsStreamer)
                        {
                            MainWindow.instance.streamerChatConnection.SendChatMessage(response);
                        }
                        else
                        {
                            MainWindow.instance.botChatConnection.SendChatMessage(response);
                            MainWindow.colChatMessages.Add(new TwitchChatMessage(
                                MainWindow.instance.accountBot.UserName, response));
                        }
                    }
                }  
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

        public string Response
        {
            get
            {
                return response;
            }
        }

        #endregion

    }
}
