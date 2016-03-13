using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace OakBot
{
    public class Song : INotifyPropertyChanged
    {
        private string songName;
        private string link;
        private SongType type;

        public event PropertyChangedEventHandler PropertyChanged;

        public string SongName
        {
            get
            {
                return songName;
            }
        }

        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;
                OnPropertyChanged(Link);
            }
        }

        public SongType Type
        {
            get
            {
                return type;
            }
        }

        public Song(string link)
        {
            string tmpLink = link;
            if (Regex.IsMatch(link, "soundcloud", RegexOptions.IgnoreCase))
            {
                type = SongType.SOUNDCLOUD;
                songName = "Song from Soundcloud";
            }
            else if (Regex.IsMatch(link, @"^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$", RegexOptions.IgnoreCase))
            {
                type = SongType.YOUTUBE_LINK;
                Thread sn = new Thread(new ThreadStart(delegate ()
                {
                    songName = Utils.getTitleFromYouTube(link);
                    OnPropertyChanged(SongName);
                }));
                sn.Start();
            }
            else
            {
                type = SongType.INVALID;
            }
            Link = tmpLink;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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