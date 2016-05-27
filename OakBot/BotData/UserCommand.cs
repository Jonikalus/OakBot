using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OakBot
{
    public enum Rank
    {
        VIEWER,
        REGULAR,
        VIP1,
        VIP2,
        VIP3,
        MODERATOR,
        BOT
    }

    public class UserCommand : INotifyPropertyChanged
    {
        #region Private Fields

        private string command;

        private int costMod;

        private int costRegular;

        private int costViewer;

        private int costVIP1;

        private int costVIP2;

        private int costVIP3;

        private Dictionary<string, DateTime> dictLastUsed = new Dictionary<string, DateTime>();

        private bool enabled;

        private int gCooldownSec;

        private DateTime lastUsed;

        private Rank rankRequired;

        private string response;

        private bool sendAsStreamer;

        private int uCooldownSec;

        private string var1;

        private string var2;

        private string var3;

        #endregion Private Fields

        #region Public Constructors

        public UserCommand(string command, string response, int gCooldownSec,
            int uCooldownSec, bool enabled, bool sendAsStreamer = false)
        {
            this.command = command.ToLower();
            this.response = response;
            this.gCooldownSec = gCooldownSec;
            this.uCooldownSec = uCooldownSec;
            this.enabled = enabled;
            this.sendAsStreamer = sendAsStreamer;

            this.costViewer = 0;
            this.costRegular = 0;
            this.costVIP1 = 0;
            this.costVIP2 = 0;
            this.costVIP3 = 0;
            this.costMod = 0;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public string Command
        {
            get
            {
                return command;
            }
        }

        public string LastUsed
        {
            get
            {
                if (lastUsed == DateTime.MinValue)
                {
                    return "Never";
                }
                else
                {
                    return lastUsed.ToString("yyyy-MM-dd hh:mm");
                }
            }
        }

        public string Response
        {
            get
            {
                return response;
            }
        }

        public string Status
        {
            get
            {
                if (enabled)
                {
                    return "Enabled";
                }
                else
                {
                    return "Disabled";
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void ExecuteCommand(IrcMessage message)
        {
            // Get command user's Viewer object
            Viewer viewer = MainWindow.colDatabase.FirstOrDefault(x => x.UserName == message.Author);
            if (viewer == null)
            {
                return;
            }

            if (CanExecute(viewer))
            {
                Match targetMatch = Regex.Match(message.Message, @"@(?<name>[a-zA-Z0-9_]{4,25})");
                string target = targetMatch.Groups["name"].Value;

                // Parse response line here
                // @user@ -> Display name of the user of the command

                // @followdate@ -> Command user's follow date
                // @followdatetime@ -> Command user's follow date and time

                // @game@ -> Channels current game
                // @title@ -> Channels current title

                string parsedResponse = Regex.Replace(response, @"@(?<item>\w+)@", m =>
                {
                    string[] split = Regex.Split(message.Message, @"\s+");

                    switch (m.Groups["item"].Value.ToLower())
                    {
                        case "user":
                            return viewer.UserName;

                        case "followdate":
                            return viewer.GetFollowDateTime("yyyy-MM-dd");

                        case "followdatetime":
                            return viewer.GetFollowDateTime("yyyy-MM-dd HH:mm");

                        case "game":
                            return Utils.GetClient().GetMyChannel().Game;

                        case "title":
                            return Utils.GetClient().GetMyChannel().Status;

                        case "var1":
                            if (split.Count() == 2)
                            {
                                var1 = split[1];
                                return var1;
                            }
                            return "";

                        default:
                            return "";
                    }
                });

                // Set timestamps
                lastUsed = DateTime.UtcNow;
                dictLastUsed[message.Author] = DateTime.UtcNow;

                // Notify the UI of the new last used date
                NotifyPropertyChanged("LastUsed");

                // Send the response
                if (sendAsStreamer)
                {
                    // Send message to IRC, no need to add to collection as this will be received by bot account
                    MainWindow.instance.streamerChatConnection.SendChatMessage(parsedResponse.Trim());
                }
                else
                {
                    // Send message to IRC
                    MainWindow.instance.botChatConnection.SendChatMessage(parsedResponse.Trim());

                    // Add message to collection
                    App.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        MainWindow.colChatMessages.Add(new IrcMessage(
                            MainWindow.instance.accountBot.UserName, parsedResponse.Trim()));
                    }));
                }
            }
        }

        public void ExecuteCommandDiscord(Message message)
        {
            MainWindow.popMsgBox(message.User.Id.ToString());
            Viewer viewer;
            try
            {
                viewer = MainWindow.colDatabase.FirstOrDefault(x => x.DiscordID == message.User.Id.ToString());
                MainWindow.popMsgBox(viewer.UserName);
            }
            catch (Exception)
            {
                viewer = new Viewer(message.User.Name);
                MainWindow.popMsgBox(viewer.UserName);
            }
            if (viewer == null)
            {
                return;
            }

            if (CanExecute(viewer))
            {
                Match targetMatch = Regex.Match(message.Text, @"@(?<name>[a-zA-Z0-9_]{4,25})");
                string target = targetMatch.Groups["name"].Value;

                // Parse response line here
                // @user@ -> Display name of the user of the command

                // @followdate@ -> Command user's follow date
                // @followdatetime@ -> Command user's follow date and time

                // @game@ -> Channels current game
                // @title@ -> Channels current title

                string parsedResponse = Regex.Replace(response, @"@(?<item>\w+)@", m =>
                {
                    string[] split = Regex.Split(message.Text, @"\s+");

                    switch (m.Groups["item"].Value.ToLower())
                    {
                        case "user":
                            return viewer.UserName;

                        case "followdate":
                            return viewer.GetFollowDateTime("yyyy-MM-dd");

                        case "followdatetime":
                            return viewer.GetFollowDateTime("yyyy-MM-dd HH:mm");

                        case "game":
                            return Utils.GetClient().GetMyChannel().Game;

                        case "title":
                            return Utils.GetClient().GetMyChannel().Status;

                        case "var1":
                            if (split.Count() == 2)
                            {
                                var1 = split[1];
                                return var1;
                            }
                            return "";

                        default:
                            return "";
                    }
                });

                MainWindow.discord.GetServer(message.Server.Id).GetChannel(message.Channel.Id).SendMessage(parsedResponse.Trim());
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool CanExecute(Viewer viewer)
        {
            // Check if command user has PERMISSION to use the command
            if (true)
            {
                // Check if command is on GLOBAL COOLDOWN
                if (gCooldownSec == 0 || DateTime.Now.Subtract(lastUsed).TotalSeconds > gCooldownSec)
                {
                    // Add user key if not exists to prevent exceptions if key not exists
                    // and it saves us another check when setting new timestamp, as this
                    // will already make sure they key exists.
                    if (!dictLastUsed.ContainsKey(viewer.UserName))
                    {
                        dictLastUsed.Add(viewer.UserName, DateTime.MinValue);
                    }

                    // Check if command user is not USER COOLDOWN
                    if (uCooldownSec == 0 || DateTime.Now.Subtract(dictLastUsed[viewer.UserName]).TotalSeconds > uCooldownSec)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion Private Methods
    }
}