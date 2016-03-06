using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OakBot.BotData
{
    public class Group : List<Viewer>, INotifyPropertyChanged
    {
        private string groupName;

        public string GroupName
        {
            get
            {
                return groupName;
            }
            set
            {
                groupName = value;
                OnPropertyChanged(GroupName);
            }
        }



        public Group(string groupName)
        {
            this.groupName = groupName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
