using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace OakBot
{
    class Config
    {
        public static string StreamerOAuthKey { get; set; }
        public static string BotOAuthKey { get; set; }
        public static string StreamerUsername { get; set; }
        public static string BotUsername { get; set; }

        public static void GetConfigFromDb()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3;");
            conn.Open();
            string sql = "SELECT * FROM oak_settings";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                StreamerOAuthKey = (string)reader["StreamerOAuthToken"];
                BotOAuthKey = (string)reader["BotOAuthToken"];
                StreamerUsername = (string)reader["StreamerTwitchUsername"];
                BotUsername = (string)reader["BotTwitchUsername"];
            }
            conn.Close();
        }

        public static void SaveConfigToDb()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3;");
            conn.Open();
            string sql = string.Format("UPDATE oak_settings SET StreamerOAuthToken = {0}, BotOAuthToken = {1}, StreamerTwitchUsername = {2}, BotTwitchUsername = {3}", StreamerOAuthKey, BotOAuthKey, StreamerUsername, BotUsername);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
    }
}
