using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;

namespace OakBot
{
    public static class DatabaseUtils
    {
        public static readonly string fileViewers = Config.AppDataPath + "\\OakBotViewers.sqlite";
        public static readonly string fileQuotes = Config.AppDataPath + "\\OakBotQuotes.sqlite";
        public static readonly string fileCommands = Config.AppDataPath + "\\OakBotCommands.sqlite";
        public static readonly string fileCurrency = Config.AppDataPath + "\\OakBotCurrency.sqlite";


        #region Database Viewers

        /// <summary>
        /// Load viewers from DBfile to colDatabase.
        /// </summary>
        public static void LoadAllViewers()
        {
            SQLiteConnection dbConnection;

            // Create new database-file and table if not exists
            if (!File.Exists(fileViewers))
            {
                SQLiteConnection.CreateFile(fileViewers);

                dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3;", fileViewers));
                dbConnection.Open();

                SQLiteCommand sqlCmd = new SQLiteCommand("CREATE TABLE `Viewers` (`Username` TEXT NOT NULL, `Points` INTEGER, `Spent` INTEGER, `Watched` TEXT, `LastSeen` TEXT, `Raids` INTEGER, `Title` TEXT, `Regular` BOOLEAN, `IGN` TEXT, PRIMARY KEY(Username))", dbConnection);
                sqlCmd.ExecuteNonQuery();

                dbConnection.Close();
            }
            // Load database-file otherwise
            else
            {
                dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3; Read Only=True;", fileViewers));
                dbConnection.Open();

                SQLiteCommand read = new SQLiteCommand("SELECT * FROM `Viewers`", dbConnection);
                SQLiteDataReader reader = read.ExecuteReader();

                while (reader.Read())
                {
                    Viewer loadedViewer = new Viewer((string)reader["Username"]);
                    loadedViewer.Points = (long)reader["Points"];
                    loadedViewer.Watched = TimeSpan.Parse((string)reader["Watched"]);
                    loadedViewer.LastSeen = DateTime.Parse((string)reader["LastSeen"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    loadedViewer.Raids = (long)reader["Raids"];
                    loadedViewer.Title = (string)reader["Title"];
                    loadedViewer.regular = (bool)reader["Regular"];
                    loadedViewer.IGN = (string)reader["IGN"];

                    MainWindow.colDatabase.Add(loadedViewer);
                }

                dbConnection.Close();
            }
        }

        /// <summary>
        /// Add all viewers to DBfile in colDatabase.
        /// Does not check if viewers already exists, make sure they don't prior calling.
        /// </summary>
        public static void AddAllViewers()
        {
            SQLiteCommand sqlCmd;

            // Open DBfile
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            // Insert new Viewer in `Viewers`
            foreach (Viewer viewer in MainWindow.colDatabase)
            {
                sqlCmd = new SQLiteCommand(
                    string.Format("INSERT INTO `Viewers` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                        viewer.UserName,
                        viewer.Points,
                        viewer.Spent,
                        viewer.Watched.ToString(),
                        viewer.LastSeen.ToString("o"),
                        viewer.Raids,
                        viewer.Title,
                        viewer.regular.ToString(),
                        viewer.IGN),
                    dbConnection);
                sqlCmd.ExecuteNonQuery();
            }

            // Close DBfile
            dbConnection.Close();
        }

        /// <summary>
        /// Update all viewers in DBfile from colDatabase.
        /// Does not check if viewers exists, make sure they do prior calling.
        /// </summary>
        public static void UpdateAllViewers()
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            SQLiteCommand sqlCmd;

            foreach(Viewer viewer in MainWindow.colDatabase)
            {
                sqlCmd = new SQLiteCommand(
                    string.Format("UPDATE `Viewers` SET `Points` = '{1}', `Spent` = '{2}', `Watched` = '{3}', `LastSeen` = '{4}', `Raids` = '{5}', `Title` = '{6}', `Regular` = '{7}', `IGN` = '{8}' WHERE `Username` = '{0}'",
                        viewer.UserName,
                        viewer.Points,
                        viewer.Spent,
                        viewer.Watched.ToString(),
                        viewer.LastSeen.ToString("o"),
                        viewer.Raids,
                        viewer.Title,
                        viewer.regular.ToString(),
                        viewer.IGN),
                dbConnection);

                sqlCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }

        /// <summary>
        /// Remove all viewers from DBfile
        /// </summary>
        public static void RemoveAllViewers()
        {
            SQLiteCommand sqlCmd;

            // Open DBfile
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            // Delete all rows in `Viewers`
            sqlCmd = new SQLiteCommand("DELETE FROM `Viewers`", dbConnection);
            sqlCmd.ExecuteNonQuery();

            // Close DBfile
            dbConnection.Close();
        }

        /// <summary>
        /// Add new viewer to DBfile with specified Viewer.
        /// Does not check if viewer exists, make sure it doesn't prior calling.
        /// </summary>
        /// <param name="viewer">Viewer to be added</param>
        public static void AddViewer(Viewer viewer)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            SQLiteCommand sqlCmd = new SQLiteCommand(
                string.Format("INSERT INTO `Viewers` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                    viewer.UserName,
                    viewer.Points,
                    viewer.Spent,
                    viewer.Watched.ToString(),
                    viewer.LastSeen.ToString("o"),
                    viewer.Raids,
                    viewer.Title,
                    viewer.regular.ToString(),
                    viewer.IGN),
                dbConnection);
            sqlCmd.ExecuteNonQuery();

            dbConnection.Close();
        }

        /// <summary>
        /// Update viewer in the DBfile with specified Viewer.
        /// Does not check if viewer exists, make sure it does prior calling.
        /// </summary>
        /// <param name="viewer">Viewer to be updated</param>
        public static void UpdateViewer(Viewer viewer)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            SQLiteCommand sqlCmd = new SQLiteCommand(
                string.Format("UPDATE `Viewers` SET `Points` = '{1}', `Spent` = '{2}', `Watched` = '{3}', `LastSeen` = '{4}', `Raids` = '{5}', `Title` = '{6}', `Regular` = '{7}', `IGN` = '{8}' WHERE `Username` = '{0}'",
                    viewer.UserName,
                    viewer.Points,
                    viewer.Spent,
                    viewer.Watched.ToString(),
                    viewer.LastSeen.ToString("o"),
                    viewer.Raids,
                    viewer.Title,
                    viewer.regular.ToString(),
                    viewer.IGN),
            dbConnection);
            sqlCmd.ExecuteNonQuery();

            dbConnection.Close();
        }

        /// <summary>
        /// Remove viewer from DBfile specified by Viewer.
        /// Does not check if viewer exists, make sure it does prior calling.
        /// </summary>
        /// <param name="viewer">Viewer to be removed</param>
        public static void RemoveViewer(Viewer viewer)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileViewers));
            dbConnection.Open();

            SQLiteCommand sqlCmd = new SQLiteCommand(string.Format("DELETE FROM `Viewers` WHERE `Username` = '{0}'", viewer.UserName), dbConnection);
            sqlCmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        #endregion

        #region Database Quotes

        // Table `Quotes` contains: `Id`, `Quote`, `Quoter`, `Date`, `DisplayDate`, `Game`, `DisplayGame`, and `LastDisplayed`

        /// <summary>
        /// Load viewers from DBfile to colDatabase.
        /// </summary>
        public static void LoadAllQuotes()
        {
            SQLiteConnection dbConnection;

            // Create new database-file and table if not exists
            if (!File.Exists(fileQuotes))
            {
                SQLiteConnection.CreateFile(fileQuotes);

                dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3;", fileQuotes));
                dbConnection.Open();

                SQLiteCommand sqlCmd = new SQLiteCommand("CREATE TABLE `Quotes` (`Id` INTERGER NOT NULL, `Quote` TEXT, `Quoter` TEXT, `Date` TEXT, `DisplayDate` BOOLEAN, `Game` TEXT, `DisplayGame` BOOLEAN, `LastDisplayed` TEXT, PRIMARY KEY(Id))", dbConnection);
                sqlCmd.ExecuteNonQuery();

                dbConnection.Close();
            }
            // Load database-file otherwise
            else
            {
                dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3; Read Only=True;", fileQuotes));
                dbConnection.Open();

                SQLiteCommand read = new SQLiteCommand("SELECT * FROM `Quotes` ORDER BY `Id` ASC", dbConnection);
                SQLiteDataReader reader = read.ExecuteReader();

                while (reader.Read())
                {
                    Quote loadedQuote = new Quote(   
                        (string)reader["Quote"],
                        (string)reader["Quoter"],
                        DateTime.Parse((string)reader["Date"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                        (bool)reader["DisplayDate"],
                        (string)reader["Game"],
                        (bool)reader["DisplayGame"],
                        DateTime.Parse((string)reader["Date"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                    );

                    MainWindow.colQuotes.Add(loadedQuote);
                }

                dbConnection.Close();
            }
        }

        /// <summary>
        /// Save all quotes after deleting current entries in case of id change
        /// </summary>
        public static void SaveAllQuotes()
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileQuotes));
            dbConnection.Open();

            SQLiteCommand sqlCmd = new SQLiteCommand("DELETE FROM `Quotes`", dbConnection);
            sqlCmd.ExecuteNonQuery();

            foreach (Quote quote in MainWindow.colQuotes)
            {
                sqlCmd = new SQLiteCommand(
                    string.Format("INSERT INTO `Quotes` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                        quote.Id,
                        quote.QuoteString,
                        quote.Quoter,
                        quote.Date.ToString("o"),
                        quote.DisplayDate,
                        quote.Game,
                        quote.DisplayGame,
                        quote.LastDisplayed.ToString("o")),
                dbConnection);
                sqlCmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }

        /// <summary>
        /// Add new quote to DBfile with specified Quote.
        /// Does not check if quote exists, make sure it doesn't prior calling.
        /// </summary>
        /// <param name="newQuote">The new quote to be added to the database</param>
        public static void AddQuote(Quote newQuote)
        {
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3", fileQuotes));
            dbConnection.Open();

            SQLiteCommand sqlCmd = new SQLiteCommand(
                string.Format("INSERT INTO `Quotes` VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                    newQuote.Id,
                    newQuote.QuoteString,
                    newQuote.Quoter,
                    newQuote.Date.ToString("o"),
                    newQuote.DisplayDate,
                    newQuote.Game,
                    newQuote.DisplayGame,
                    newQuote.LastDisplayed.ToString("o")),
                dbConnection);
            sqlCmd.ExecuteNonQuery();

            dbConnection.Close();
        }

        #endregion

        #region Currency



        #endregion Currency

    }
}
