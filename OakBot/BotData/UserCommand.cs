using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        private string command;
        private string response;
        private bool enabled;
        private bool sendAsStreamer;


        private int gCooldownSec;
        private int uCooldownSec;

        private int costViewer;
        private int costRegular;
        private int costVIP1;
        private int costVIP2;
        private int costVIP3;
        private int costMod;

        private string var1;
        private string var2;
        private string var3;

        private Rank rankRequired;

        private DateTime lastUsed;
        private Dictionary<string, DateTime> dictLastUsed = new Dictionary<string, DateTime>();

        #endregion

        #region Constructor

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

        #endregion

        #region Methods

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void ExecuteCommand(TwitchChatMessage message)
        {
            // Check if not on global cooldown
            // using Substract on DateTime creates TimeSpan which as TotalSeconds
            if (gCooldownSec == 0 || DateTime.Now.Subtract(lastUsed).TotalSeconds > gCooldownSec)
            {
                
                // Add user key if not exists to prevent exceptions if key not exists
                // and it saves us another check when setting new timestamp, as this
                // will already make sure they key exists.
                if (!dictLastUsed.ContainsKey(message.Author))
                {
                    dictLastUsed.Add(message.Author, DateTime.MinValue);
                }

                // Check if user is not on cooldown
                if (uCooldownSec == 0 || DateTime.Now.Subtract(dictLastUsed[message.Author]).TotalSeconds > uCooldownSec)
                {
                    // Get viewer's Viewer object
                    Viewer viewer = MainWindow.colDatabase.FirstOrDefault(x => x.UserName == message.Author);
                    Match targetMatch = Regex.Match(message.Message, @"@(?<name>[a-zA-Z0-9_]{4,25})");
                    string target = targetMatch.Groups["name"].Value;
                    // Check rank
                    if (true)
                    {
                        // Block error to prevent output when no arguments are given
                        bool blockError = false;

                        // Parse response line here
                        // @user@ -> Display name of the user of the command
                        // @target@
                        // @realtarget@

                        // @block@ will not output anything if no arguments are given with the command

                        string parsedResponse = Regex.Replace(response, @"@(?<item>\w+)@", m =>
                        {
                            string[] split = Regex.Split(message.Message, @"\s+");
                            
                            switch (m.Groups["item"].Value.ToLower())
                            {
                                case "user":
                                    return viewer.UserName;

                                case "block":
                                    if(split.Count() == 1)
                                    {
                                        blockError = true;
                                    }
                                    return "";

                                case "followdate":
                                    return viewer.GetFollowDateTime("yyyy-MM-dd");

                                case "followdatetime":
                                    return viewer.GetFollowDateTime("yyyy-MM-dd HH:mm");

                                case "var1":
                                    if (split.Count() == 2)
                                    {
                                        var1 = split[1];
                                        return var1;
                                    }
                                    return "";
                                case "songrequest":
                                    if (split.Count() == 2)
                                    {
                                        string link = split[1];
                                        Song request = new Song(link);
                                        if(request.Type != SongType.INVALID)
                                        {
                                            MainWindow.colSongs.Add(request);
                                            string name = Utils.getTitleFromYouTube(link);
                                            return name;
                                        }
                                        
                                    }
                                    return "Sorry, invalid song!";
                                case "song":
                                    if (MainWindow.playState)
                                    {
                                        return MainWindow.colSongs[MainWindow.indexSong].SongName;
                                    }else
                                    {
                                        return "None";
                                    }

                                case "target":
                                    return target;
                                default:
                                    return "CMD-DOES-NOT-EXIST";
                            }
                            
                        });

                        // Continue if there is no @block@ error
                        if (!blockError)
                        {
                            // Set timestamps
                            lastUsed = DateTime.UtcNow;
                            dictLastUsed[message.Author] = DateTime.UtcNow;

                            // Notify the UI of the new last used date
                            NotifyPropertyChanged("LastUsed");

                            // Send the response
                            if (sendAsStreamer)
                            {
                                MainWindow.instance.streamerChatConnection.SendChatMessage(parsedResponse.Trim());
                            }
                            else
                            {
                                MainWindow.instance.botChatConnection.SendChatMessage(parsedResponse.Trim());
                                MainWindow.colChatMessages.Add(new TwitchChatMessage(
                                    MainWindow.instance.accountBot.UserName, parsedResponse.Trim()));
                            }
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

        #endregion

    }
}
