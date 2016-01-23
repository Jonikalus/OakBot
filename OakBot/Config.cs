using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;

namespace OakBot
{
    class Config
    {
        // Twitch Application
        public static string twitchClientID {
            get
            {
                return "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
            }
        }

        private static string streamerOAuthKey;
        private static string botOAuthKey;
        private static string streamerUsername;
        private static string botUsername;
        private static string channelName;
        private static string server;
        private static int port;
        private static bool autoConnect;

        public static string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";
        public static string StreamerOAuthKey {
            get
            {
                return streamerOAuthKey;
            }
            set
            {
                streamerOAuthKey = value;
                SaveConfigToDb();
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
                SaveConfigToDb();
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
                SaveConfigToDb();
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
                SaveConfigToDb();
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
                SaveConfigToDb();
            }
        }

        public static string Server {
            get
            {
                return server;
            }
            set
            {
                server = value;
                SaveConfigToDb();
            }
        }

        public static int Port {
            get
            {
                return port;
            }
            set
            {
                port = value;
                SaveConfigToDb();
            }
        }

        public static bool AutoConnect {
            get
            {
                return autoConnect;
            }
            set
            {
                autoConnect = value;
                SaveConfigToDb();
            }
        }

        

        public static void GetConfigFromDb()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;");
                conn.Open();
                string sql = "SELECT * FROM oak_settings";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    switch ((string)reader["name"])
                    {
                        case "BotTwitchUsername":
                            botUsername = (string)reader["value"];
                            break;
                        case "StreamerTwitchUsername":
                            streamerUsername = (string)reader["value"];
                            break;
                        case "BotOAuthToken":
                            botOAuthKey = (string)reader["value"];
                            break;
                        case "StreamerOAuthToken":
                            streamerOAuthKey = (string)reader["value"];
                            break;
                        case "DefaultChannelName":
                            channelName = (string)reader["value"];
                            break;
                        case "DefaultServer":
                            server = (string)reader["value"];
                            break;
                        case "Port":
                            port = int.Parse((string)reader["value"]);
                            break;
                        case "AutoConnect":
                            autoConnect = bool.Parse((string)reader["value"]);
                            break;
                        default:
                            break;
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public static void CreateDatabaseIfNotExist()
        {
            if (!File.Exists("OakSettings.sqlite"))
            {
                try
                {
                    SQLiteConnection.CreateFile("OakSettings.sqlite");
                    SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3");
                    conn.Open();
                    string creationSql = "CREATE TABLE IF NOT EXISTS `oak_settings` ( `name` TEXT NOT NULL, `value` TEXT NOT NULL, PRIMARY KEY(name) )";
                    string strOAuth = "INSERT INTO `oak_settings` VALUES ( 'StreamerOAuthToken', 'please change' )";
                    string botOAuth = "INSERT INTO `oak_settings` VALUES ( 'BotOAuthToken', 'please change' )";
                    string strUser = "INSERT INTO `oak_settings` VALUES ( 'StreamerTwitchUsername', 'please change' )";
                    string botUser = "INSERT INTO `oak_settings` VALUES ( 'BotTwitchUsername', 'please change' )";
                    string channelName = "INSERT INTO `oak_settings` VALUES ( 'DefaultChannelName', '' )";
                    string server = "INSERT INTO `oak_settings` VALUES ( 'DefaultServer', 'tmi.twitch.tv' )";
                    string port = "INSERT INTO `oak_settings` VALUES ( 'Port', '6667' )";
                    string autoConnect = "INSERT INTO `oak_settings` VALUES ( 'AutoConnect', 'false' )";
                    SQLiteCommand cmd = new SQLiteCommand(creationSql, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(strOAuth, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(botOAuth, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(strUser, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(botUser, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(channelName, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(server, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(port, conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand(autoConnect, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            
        }

        public static void SaveConfigToDb()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3;");
            conn.Open();
            //string sql = string.Format("UPDATE oak_settings SET StreamerOAuthToken = '{0}', BotOAuthToken = '{1}', StreamerTwitchUsername = '{2}', BotTwitchUsername = '{3}'", StreamerOAuthKey, BotOAuthKey, StreamerUsername, BotUsername);
            string sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", BotUsername, "BotTwitchUsername");
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", StreamerUsername, "StreamerTwitchUsername");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", BotOAuthKey, "BotOAuthToken");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", StreamerOAuthKey, "StreamerOAuthToken");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", ChannelName, "DefaultChannelName");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", Server, "DefaultServer");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", Port, "Port");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", AutoConnect, "AutoConnect");
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static bool ImportFromAnkhbot(MainWindow _mW)
        {
            // Create OpenFileDialog and set default file extention and filters
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sqlite";
            dlg.Filter = "AnkhBot CurrencyDB|CurrencyDB.sqlite";
            dlg.InitialDirectory = @"%appdata%\AnkhHeart\AnkhBotR2\Twitch\Databases";

            // Show file dialog
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                int counter = 0;
                string connString = string.Format("DataSource={0}; Version=3; Read Only=True;", dlg.FileName);

                try
                {
                    SQLiteConnection dbConnection = new SQLiteConnection(connString);
                    dbConnection.Open();

                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM CurrencyUser", dbConnection);
                    SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        TwitchUser viewer = new TwitchUser((string)dataReader["Name"]);

                        viewer.rank = (string)dataReader["Rank"];
                        viewer.points = (long)dataReader["Points"];
                        viewer.raids = (long)dataReader["Raids"];
                        viewer.dateLastSeen = DateTime.Parse((string)dataReader["LastSeen"]);

                        // AnkhBot's time format d.HH:MM:SS where d is not present if < 1 day
                        string ankhbotHours = (string)dataReader["Hours"];
                        TimeSpan watchedHours = new TimeSpan();
                        if (ankhbotHours.Contains("."))
                        {
                            TimeSpan.TryParseExact(ankhbotHours, @"d\.hh\:mm\:ss", null, out watchedHours);
                        }
                        else
                        {
                            TimeSpan.TryParseExact(ankhbotHours, @"hh\:mm\:ss", null, out watchedHours);
                        }
                        viewer.watchedTimeSpan = watchedHours;

                        MainWindow.viewerDatabase.Add(viewer);
                        counter++;
                    }

                    dbConnection.Close();
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(string.Format("Could not open or read the selected sqlite database file.\n\n{0}", ex.ToString()),
                        "AnkhBot User Data Import", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("The following program error has occured:\n\n{0}", ex.ToString()),
                        "AnkhBot User Data Import", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                // Succesfull import
                MessageBox.Show(string.Format("Completed import from AnkhBot.\nAdded {0} records.", counter),
                    "AnkhBot Import", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }

            // User canceled the file selection
            return false;

        }
    }
}
