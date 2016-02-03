using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Globalization;

namespace OakBot
{
    public static class ViewerDB
    {
        public static void GetViewersFromDB()
        {
            string filename = Config.AppDataPath + @"\OakBotViewers.sqlite";
            SQLiteConnection dbConnection;
            if (!File.Exists(filename))
            {
                SQLiteConnection.CreateFile(filename);

                dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
                dbConnection.Open();

                SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE `Viewers` (`Username` TEXT NOT NULL, `Points` INTEGER, `Raids` INTEGER, `Rank` TEXT, `Watched` TEXT, `LastSeen` TEXT, `Regular` BOOLEAN, PRIMARY KEY(Username))", dbConnection);
                cmd.ExecuteNonQuery();

                dbConnection.Close();
            }

            dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
            dbConnection.Open();

            SQLiteCommand read = new SQLiteCommand("SELECT * FROM `Viewers`", dbConnection);
            SQLiteDataReader reader = read.ExecuteReader();

            while (reader.Read())
            {
                MainWindow.colDatabase.Add(new TwitchViewer((string)reader["Username"], (int)reader["Points"], (int)reader["Raids"], (string)reader["Rank"], TimeSpan.FromMinutes((double)reader["Watched"]), DateTime.Parse((string)reader["LastSeen"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), (bool)reader["Regular"]));
            }

            dbConnection.Clone();
        }

        public static void SaveViewersToDB()
        {
            string filename = Config.AppDataPath + @"\OakBotViewers.sqlite";
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
            dbConnection.Open();
            SQLiteCommand cmd;
            foreach(TwitchViewer viewer in MainWindow.colDatabase)
            {
                cmd = new SQLiteCommand(string.Format("UPDATE `Viewers` SET `Points` = {1}, `Raids` = {2}, `Rank` = {3}, `Watched` = {4}, `LastSeen` = {5}, `Regular` = {6} WHERE `Username` = {0}", viewer.UserName, viewer.Points, viewer.Raids, viewer.Rank, viewer.watchedTimeSpan.Minutes, viewer.dateLastSeen.ToString("o", CultureInfo.InvariantCulture), viewer.regular.ToString()), dbConnection);
                cmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }

        public static void AddNewViewerToDB(TwitchViewer viewer)
        {
            string filename = Config.AppDataPath + @"\OakBotViewers.sqlite";
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
            dbConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(string.Format("INSERT INTO `Viewers` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", viewer.UserName, viewer.Points, viewer.Raids, viewer.Rank, viewer.watchedTimeSpan.Minutes, viewer.dateLastSeen.ToString("o", CultureInfo.InvariantCulture), viewer.regular.ToString()), dbConnection);
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        public static void RemoveViewerFromDB(TwitchViewer viewer)
        {
            string filename = Config.AppDataPath + @"\OakBotViewers.sqlite";
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
            dbConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(string.Format("DELETE FROM `Viewers` * WHERE `Username` = {0}", viewer.UserName), dbConnection);
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        public static void UpdateViewerToDB(TwitchViewer viewer)
        {
            string filename = Config.AppDataPath + @"\OakBotViewers.sqlite";
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3", filename));
            dbConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(string.Format("UPDATE `Viewers` SET `Points` = {1}, `Raids` = {2}, `Rank` = {3}, `Watched` = {4}, `LastSeen` = {5}, `Regular` = {6} WHERE `Username` = {0}", viewer.UserName, viewer.Points, viewer.Raids, viewer.Rank, viewer.watchedTimeSpan.Minutes, viewer.dateLastSeen.ToString("o", CultureInfo.InvariantCulture), viewer.regular.ToString()), dbConnection);
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
    }
}
