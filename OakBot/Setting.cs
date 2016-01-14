using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
