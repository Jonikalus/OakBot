using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OakBot
{
    public class Song :INotifyPropertyChanged
    {
        private string songName;
        private string link;
        private SongType type;

        public event PropertyChangedEventHandler PropertyChanged;

        public string SongName {
            get
            {
                return songName;
            }
            set
            {
                songName = value;
                NotifyPropertyChanged("SongName");
            }
        }
        public string Link {
            get
            {
                return link;
            }
            set
            {
                link = value;
                NotifyPropertyChanged("Link");
            }
        }
        public SongType Type {
            get
            {
                return type;
            }
        }

        public Song(string songname, string link)
        {
            SongName = songname;
            string tmpLink = link;
            if (Regex.IsMatch(link, "soundcloud", RegexOptions.IgnoreCase))
            {
                type = SongType.SOUNDCLOUD;
            }else if(Regex.IsMatch(link, "youtube", RegexOptions.IgnoreCase))
            {
                type = SongType.YOUTUBE_LINK;
            }else
            {
                type = SongType.INVALID;
            }
            Link = tmpLink;
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public enum SongType
    {
        YOUTUBE,
        YOUTUBE_LINK,
        SOUNDCLOUD,
        INVALID
    }
}
