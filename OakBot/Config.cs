using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace OakBot
{
    class Config : IEnumerable<Setting>
    {
        private List<Setting> conf;

        public IEnumerator<Setting> GetEnumerator()
        {
            return conf.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Config()
        {
            conf = new List<Setting>();
            SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3;");
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM oak_settings", conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name, value;
                name = (string)reader["name"];
                value = (string)reader["value"];
                conf.Add(new Setting((SettingType)Enum.Parse(typeof(SettingType), name), value));
            }
        }

        public void Save()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=OakSettings.sqlite;Version=3;");
            foreach (Setting s in this)
            {
                SQLiteCommand cmd = new SQLiteCommand(string.Format("UPDATE oak_settings SET value = {0} WHERE name = {1}", s.Value, s.SettingType.ToString("F")), conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
