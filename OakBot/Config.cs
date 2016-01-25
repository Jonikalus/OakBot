using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.IO;

namespace OakBot
{
    class Config
    {
        // Methods
        public static void GetConfigFromDb()
        {
            // Set filename
            string filename = "OakBotDatabase.sqlite";

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
                        "('AutoConnectStreamer', 'false') ", dbConnection);
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
        public static void SaveConfigToDb(string settingName, string settingsValue)
        {
            try
            {
                // Set filename
                string filename = "OakBotDatabase.sqlite";

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

        // Twitch Application
        public static string twitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        public static string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        // Settings fields and properties
        private static string streamerOAuthKey;
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
                    SaveConfigToDb("StreamerOAuthToken", value);
                }
            }
        }

        private static string botOAuthKey;
        public static string BotOAuthKey {
            get
            {
                return botOAuthKey;
            }
            set
            {
                if (value != botOAuthKey)
                {
                    botOAuthKey = value;
                    SaveConfigToDb("BotOAuthToken", value);
                }
            }
        }

        private static string streamerUsername;
        public static string StreamerUsername {
            get
            {
                return streamerUsername;
            }
            set
            {
                if (value != streamerUsername)
                {
                    streamerUsername = value;
                    SaveConfigToDb("StreamerTwitchUsername", value);
                }
            }
        }

        private static string botUsername;
        public static string BotUsername {
            get
            {
                return botUsername;
            }
            set
            {
                if (value != botUsername)
                {
                    botUsername = value;
                    SaveConfigToDb("BotTwitchUsername", value);
                }
            }
        }

        private static string channelName;
        public static string ChannelName {
            get
            {
                return channelName;
            }
            set
            {
                if (value != channelName)
                {
                    channelName = value;
                    SaveConfigToDb("ChannelName", value);
                }
            }
        }

        private static string serverIP;
        public static string ServerIP {
            get
            {
                return serverIP;
            }
            set
            {
                if (value != serverIP)
                {
                    serverIP = value;
                    SaveConfigToDb("ServerIP", value);
                }
            }
        }

        private static int serverPort;
        public static int ServerPort {
            get
            {
                return serverPort;
            }
            set
            {
                if (value != serverPort)
                {
                    serverPort = value;
                    SaveConfigToDb("ServerPort", value.ToString());
                }
            }
        }

        private static bool autoConnectBot;
        public static bool AutoConnectBot
        {
            get
            {
                return autoConnectBot;
            }
            set
            {
                if(value != autoConnectBot)
                {
                    autoConnectBot = value;
                    SaveConfigToDb("AutoConnectBot", value.ToString());
                }
            }
        }

        private static bool autoConnectStreamer;
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
                    SaveConfigToDb("AutoConnectStreamer", value.ToString());
                }
            }
        }

    }
}
