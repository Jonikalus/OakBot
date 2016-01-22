using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Media;

namespace OakBot
{
    class Config
    {
        // Twitch Application
        public static string twitchClientID = "gtpc5vtk1r4u8fm9l45f9kg1fzezrv8";
        public static string twitchClientSecret = "ss6pafrg7i0nqhgvun9y5cq4wc61ogc";

        public static string StreamerOAuthKey { get; set; }
        public static string BotOAuthKey { get; set; }
        public static string StreamerUsername { get; set; }
        public static string BotUsername { get; set; }

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
                            BotUsername = (string)reader["value"];
                            break;
                        case "StreamerTwitchUsername":
                            StreamerUsername = (string)reader["value"];
                            break;
                        case "BotOAuthToken":
                            BotOAuthKey = (string)reader["value"];
                            break;
                        case "StreamerOAuthToken":
                            StreamerOAuthKey = (string)reader["value"];
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

                        _mW.viewerDatabase.Add(viewer);
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
