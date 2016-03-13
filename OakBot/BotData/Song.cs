using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace OakBot
{
    public enum SongType
    {
        YOUTUBE,
        YOUTUBE_LINK,
        SOUNDCLOUD,
        INVALID
    }

    public class Song : INotifyPropertyChanged
    {
        #region Private Fields

        private string link;
        private string songName;
        private SongType type;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

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

        public string SongName
        {
            get
            {
                return songName;
            }
        }

        public SongType Type
        {
            get
            {
                return type;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods
    }
}