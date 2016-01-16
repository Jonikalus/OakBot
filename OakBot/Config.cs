using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace OakBot
{
    class Config : ICollection<Setting>
    {
        protected List<Setting> conf;

        

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
                Add(new Setting((SettingType)Enum.Parse(typeof(SettingType), name), value));
            }
        }

        public int Count
        {
            get
            {
                return ((ICollection<Setting>)conf).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<Setting>)conf).IsReadOnly;
            }
        }

        public void Add(Setting item)
        {
            ((ICollection<Setting>)conf).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Setting>)conf).Clear();
        }

        public bool Contains(Setting item)
        {
            return ((ICollection<Setting>)conf).Contains(item);
        }

        public void CopyTo(Setting[] array, int arrayIndex)
        {
            ((ICollection<Setting>)conf).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Setting> GetEnumerator()
        {
            return ((ICollection<Setting>)conf).GetEnumerator();
        }

        public bool Remove(Setting item)
        {
            return ((ICollection<Setting>)conf).Remove(item);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<Setting>)conf).GetEnumerator();
        }
    }
}
