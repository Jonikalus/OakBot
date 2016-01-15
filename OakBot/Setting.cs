using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    class Setting
    {
        
        public SettingType SettingType { get; set; }

        public string Value { get; set; }

        public Setting(SettingType type, string value)
        {
            SettingType = type;
            Value = value;
        }
    }

    enum SettingType
    {
        StreamerOAuthToken = 0,
        BotOAuthToken = 1,
        StreamerTwitchUsername = 2,
        BotTwitchUsername = 3
    }
}
