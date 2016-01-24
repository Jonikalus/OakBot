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
        // Twitch Application
        public static string twitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        public static string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        private static string streamerOAuthKey;
        private static string botOAuthKey;
        private static string streamerUsername;
        private static string botUsername;
        private static string channelName;
        private static string server;
        private static int port;
        private static bool autoConnectBot;
        private static bool autoConnectStreamer;

        #region Properties

        public static string StreamerOAuthKey
        {
            get
            {
                return streamerOAuthKey;
            }
            set
            {
                streamerOAuthKey = value;
                SaveConfigToDb("StreamerOAuthToken", value);
            }
        }

        public static string BotOAuthKey {
            get
            {
                return botOAuthKey;
            }
            set
            {
                botOAuthKey = value;
                SaveConfigToDb("BotOAuthToken", value);
            }
        }

        public static string StreamerUsername {
            get
            {
                return streamerUsername;
            }
            set
            {
                streamerUsername = value;
                SaveConfigToDb("StreamerTwitchUsername", value);
            }
        }

        public static string BotUsername {
            get
            {
                return botUsername;
            }
            set
            {
                botUsername = value;
                SaveConfigToDb("BotTwitchUsername", value);
            }
        }

        public static string ChannelName {
            get
            {
                return channelName;
            }
            set
            {
                channelName = value;
                SaveConfigToDb("ChannelName", value);
            }
        }

        public static string ServerIP {
            get
            {
                return server;
            }
            set
            {
                server = value;
                SaveConfigToDb("ServerIP", value);
            }
        }

        public static int ServerPort {
            get
            {
                return port;
            }
            set
            {
                port = value;
                SaveConfigToDb("ServerPort", value.ToString());
            }
        }

        public static bool AutoConnectBot
        {
            get
            {
                return autoConnectBot;
            }
            set
            {
                autoConnectBot = value;
                SaveConfigToDb("AutoConnectBot", value.ToString());
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
                autoConnectStreamer = value;
                SaveConfigToDb("AutoConnectStreamer", value.ToString());
            }
        }

        #endregion

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
                            server = (string)sqlReader["value"];
                            break;

                        case "ServerPort":
                            port = int.Parse((string)sqlReader["value"]);
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
    }
}
