using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace OakBot
{
    public class Config
    {
        #region Public Fields

        public static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OakBot";
        public static readonly string GameWispClientID = "a9f873bd2bf4d17da94e7a1c4cd8a614645ebff";
        public static readonly string GameWispClientSecret = "89ed9ed593e47c2a329f9e51d3e607e2f91c7f7";
        public static readonly string TwitchAlertsClientID = "kriqzeVxRHIbIHMPZzilYEDjFgfGkR7088gSx3KM";
        public static readonly string TwitchAlertsClientSecret = "7gGEWwG0i5E1LQDkEe9C9xwI3dgR6Y93v9kjrsYZ";
        public static readonly string TwitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        public static readonly string TwitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        #endregion Public Fields

        #region Private Fields

        private static bool autoConnectBot;
        private static bool autoConnectStreamer;
        private static string botOAuthKey;
        private static string botUsername;
        private static string channelName;
        private static string serverIP;
        private static int serverPort;
        private static string streamerOAuthKey;
        private static string streamerUsername;
        private static string discordBotToken;

        #endregion Private Fields

        // private static char commandPrefix;

        #region Public Events

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        #endregion Public Events

        #region Public Properties

        public static bool AutoConnectBot
        {
            get
            {
                return autoConnectBot;
            }
            set
            {
                if (value != autoConnectBot)
                {
                    autoConnectBot = value;
                    SaveSettingPropertyToDB("AutoConnectBot", value.ToString());
                }
            }
        }

        public static bool AutoConnectStreamer
        {
            get
            {
                return autoConnectStreamer;
            }
            set
            {
                if (value != autoConnectStreamer)
                {
                    autoConnectStreamer = value;
                    SaveSettingPropertyToDB("AutoConnectStreamer", value.ToString());
                }
            }
        }

        public static string BotOAuthKey
        {
            get
            {
                return botOAuthKey;
            }
            set
            {
                if (value != botOAuthKey)
                {
                    botOAuthKey = value;
                    SaveSettingPropertyToDB("BotOAuthToken", value);
                }
            }
        }

        public static string BotUsername
        {
            get
            {
                return botUsername;
            }
            set
            {
                if (value != botUsername)
                {
                    botUsername = value;
                    SaveSettingPropertyToDB("BotTwitchUsername", value);
                }
            }
        }

        public static string ChannelName
        {
            get
            {
                return channelName;
            }
            set
            {
                if (value != channelName && !string.IsNullOrWhiteSpace(value))
                {
                    channelName = value.Trim().ToLower();
                    SaveSettingPropertyToDB("ChannelName", value);
                    RaiseStaticPropertyChanged("ChannelName");
                }
            }
        }

        public static string ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                if (value != serverIP)
                {
                    serverIP = value;
                    SaveSettingPropertyToDB("ServerIP", value);
                }
            }
        }

        public static int ServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                if (value != serverPort)
                {
                    serverPort = value;
                    SaveSettingPropertyToDB("ServerPort", value.ToString());
                }
            }
        }

        public static string StreamerOAuthKey
        {
            get
            {
                return streamerOAuthKey;
            }
            set
            {
                if (value != streamerOAuthKey)
                {
                    streamerOAuthKey = value;
                    SaveSettingPropertyToDB("StreamerOAuthToken", value);
                }
            }
        }

        public static string StreamerUsername
        {
            get
            {
                return streamerUsername;
            }
            set
            {
                if (value != streamerUsername)
                {
                    streamerUsername = value;
                    SaveSettingPropertyToDB("StreamerTwitchUsername", value);
                }
            }
        }

        public static string DiscordBotToken
        {
            get
            {
                return discordBotToken;
            }
            set
            {
                if(value != discordBotToken)
                {
                    discordBotToken = value;
                    SaveSettingPropertyToDB("DiscordBotToken", value);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static void GetConfigFromDb()
        {
            // Set filename
            string filename = AppDataPath + @"\OakBotDatabase.sqlite";

            // Create database file if not exists
            if (!File.Exists(filename))
            {
                try
                {
                    // Create database file
                    SQLiteConnection.CreateFile(filename);

                    // Start connection to the database file
                    SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
                    dbConnection.Open();

                    // Create the settings table
                    SQLiteCommand sqlCmd = new SQLiteCommand(
                        "CREATE TABLE `Settings` (`name` TEXT NOT NULL, `value` TEXT NOT NULL, PRIMARY KEY(name))", dbConnection);
                    sqlCmd.ExecuteNonQuery();

                    // Insert default values
                    sqlCmd = new SQLiteCommand(
                        "INSERT INTO `Settings` (`name`, `value`) VALUES " +
                        "('BotTwitchUsername', 'notSet'), " +
                        "('BotOAuthToken', 'notSet'), " +
                        "('ChannelName', 'notSet'), " +
                        "('ServerIP', 'tmi.twitch.tv'), " +
                        "('ServerPort', '6667'), " +
                        "('AutoConnectBot', 'false'), " +
                        "('StreamerTwitchUsername', 'notSet'), " +
                        "('StreamerOAuthToken', 'notSet'), " +
                        "('AutoConnectStreamer', 'false')," +
                        "('DiscordBotToken', 'notSet')",
                        dbConnection);
                    sqlCmd.ExecuteNonQuery();

                    // Close database file
                    dbConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            // Read setting values from Database
            try
            {
                // Start connection to the database file
                SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};", filename));
                dbConnection.Open();

                // Get all data from 'Settings' table
                SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM `Settings`", dbConnection);
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    switch ((string)sqlReader["name"])
                    {
                        case "BotTwitchUsername":
                            botUsername = (string)sqlReader["value"];
                            break;

                        case "BotOAuthToken":
                            botOAuthKey = (string)sqlReader["value"];
                            break;

                        case "ChannelName":
                            channelName = (string)sqlReader["value"];
                            break;

                        case "ServerIP":
                            serverIP = (string)sqlReader["value"];
                            break;

                        case "ServerPort":
                            serverPort = int.Parse((string)sqlReader["value"]);
                            break;

                        case "AutoConnectBot":
                            autoConnectBot = bool.Parse((string)sqlReader["value"]);
                            break;

                        case "StreamerTwitchUsername":
                            streamerUsername = (string)sqlReader["value"];
                            break;

                        case "StreamerOAuthToken":
                            streamerOAuthKey = (string)sqlReader["value"];
                            break;

                        case "AutoConnectStreamer":
                            autoConnectStreamer = bool.Parse((string)sqlReader["value"]);
                            break;
                        case "DiscordBotToken":
                            discordBotToken = (string)sqlReader["value"];
                            break;
                        //case "CommandPrefix":
                        //    commandPrefix = ((string)sqlReader["value"])[0];
                        //    break;

                        default:
                            break;
                    }
                }
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void RaiseStaticPropertyChanged(string propName)
        {
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propName));
        }

        public static void SaveSettingPropertyToDB(string settingName, string settingsValue)
        {
            try
            {
                // Set filename
                string filename = AppDataPath + @"\OakBotDatabase.sqlite";

                // Start connection to the database file
                SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
                dbConnection.Open();

                // Update `Settings` per value
                SQLiteCommand sqlCmd = new SQLiteCommand(
                    string.Format("UPDATE `Settings` SET `value` = '{0}' WHERE `name` = '{1}'", settingsValue, settingName), dbConnection);
                sqlCmd.ExecuteNonQuery();

                // Close database file
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion Public Methods

        //public static char CommandPrefix
        //{
        //    get
        //    {
        //        return commandPrefix;
        //    }
        //    set
        //    {
        //        if(value != commandPrefix)
        //        {
        //            commandPrefix = value;
        //            SaveSettingPropertyToDB("CommandPrefix", value.ToString());
        //        }
        //    }
        //}
    }
}