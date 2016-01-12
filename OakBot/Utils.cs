using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.IO;

namespace OakBot
{
    class Utils
    {
        //Extracts the OAuth token from an URL
        public static string getAuthTokenFromUrl(string absoluteUrl)
        {
            Match url = Regex.Match(absoluteUrl, "access_token=(?<token>[a-zA-Z0-9]+)&");
            string token = url.Groups["token"].Value.Trim();
            return token;
        }

        //Creates a database by file name
        public static void CreateDatabase(string fileName)
        {
            if (!File.Exists(fileName))
            {
                SQLiteConnection.CreateFile(fileName);
            }
        }

        //Connects to database by file name
        public static SQLiteConnection ConnectDatabase(string databaseName)
        {
            return new SQLiteConnection(string.Format("Data Source={0};Version=3;", databaseName));
        }

        //Executes a given query using the connection given
        //CREATE TABLE oak_settings (name TEXT NOT NULL PRIMARY KEY, value TEXT NOT NULL) 
        public static void ExecuteQuery(string sql, SQLiteConnection connection)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        //Creates initial tables for the database
        //Initial tables:
        /*
        oak_settings
        */
        public static void CreateInitialTables(SQLiteConnection connection)
        {
            ExecuteQuery("CREATE TABLE oak_settings (name TEXT NOT NULL PRIMARY KEY, value TEXT NOT NULL)", connection);
        }

        //Writes a value to a setting based on the name (may be changed)
        public static void WriteConfigValue(string settingName, string value, SQLiteConnection connection)
        {
            ExecuteQuery(string.Format("UPDATE oak_settings SET value = '{0}' WHERE name = '{1}'", value, settingName), connection);
        }

        //Initial entries for the settings table
        public static void CreateInitialSettings(SQLiteConnection connection)
        {
            ExecuteQuery("INSERT INTO oak_settings (name, value) VALUES ('streamerOAuthToken', 'please change')", connection);
            ExecuteQuery("INSERT INTO oak_settings (name, value) VALUES ('botOAuthToken', 'please change')", connection);
            ExecuteQuery("INSERT INTO oak_settings (name, value) VALUES ('streamerUserName', 'please change')", connection);
            ExecuteQuery("INSERT INTO oak_settings (name, value) VALUES ('botUserName', 'please change')", connection);
        }
    }
}
