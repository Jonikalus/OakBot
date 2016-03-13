using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace OakBot
{
    public class Viewer : INotifyPropertyChanged
    {
        #region Private Fields

        // In-Game-Name field that viewers can set
        private string ign;

        // Timestamp of last seen
        // This will be updated each background check
        private DateTime lastSeen;

        // Points the user has
        private long points;

        // Amount of raids this user did on
        // the users channel (mostly other streamers)
        private long raids;

        // Points the user have spent
        private long spent;

        // Title earned by points/hours or bought
        private string title;

        // Username of the viewer
        private string userName;

        // Watched timespan of the viewer
        private TimeSpan watched;

        #endregion Private Fields

        #region Public Constructors

        public Viewer(string userName)
        {
            this.userName = userName;

            this.points = 0;
            this.spent = 0;
            this.watched = new TimeSpan(0);
            this.lastSeen = DateTime.UtcNow;
            this.raids = 0;
            this.title = "";
            this.regular = false;
            this.ign = "";
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public string comment { get; set; }

        public DateTime expVIP1 { get; set; }

        public DateTime expVIP2 { get; set; }

        public DateTime expVIP3 { get; set; }

        public bool forcedRegRemove { get; set; }

        public DateTime gotVIP1 { get; set; }

        public DateTime gotVIP2 { get; set; }

        public DateTime gotVIP3 { get; set; }

        public double Hours
        {
            get
            {
                return Math.Round(watched.TotalHours, 1, MidpointRounding.AwayFromZero);
            }
        }

        public string IGN
        {
            get
            {
                return ign;
            }
            set
            {
                if (value != ign)
                {
                    title = value;
                    NotifyPropertyChanged("IGN");
                }
            }
        }

        public DateTime LastSeen
        {
            get
            {
                return lastSeen;
            }
            set
            {
                lastSeen = value;
            }
        }

        public long Minutes
        {
            get
            {
                return Convert.ToInt64(watched.TotalMinutes);
            }
        }

        // JOIN and PART messages and comment field
        public string msgJoin { get; set; }

        public string msgPart { get; set; }

        public long Points
        {
            get
            {
                return points;
            }
            set
            {
                if (value != points)
                {
                    points = value;
                    NotifyPropertyChanged("Points");
                }
            }
        }

        public long Raids
        {
            get
            {
                return raids;
            }
            set
            {
                if (value != raids)
                {
                    raids = value;
                    NotifyPropertyChanged("Raids");
                }
            }
        }

        public Rank rank { get; set; }

        // Regular indicator once point/hours goal has been met
        // Won't be revoked by the bot but can be by the user
        public bool regular { get; set; }

        public long Spent
        {
            get
            {
                return spent;
            }
            set
            {
                if (value != spent)
                {
                    spent = value;
                    NotifyPropertyChanged("Spent");
                }
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }
        }

        //private List<Group> groups;
        //public List<Group> Groups
        //{
        //    get
        //    {
        //        return groups;
        //    }
        //    set
        //    {
        //        groups = value;
        //        NotifyPropertyChanged("Groups");
        //    }
        //}
        // VIP Bronze
        public bool VIP1 { get; set; }

        // VIP Silver
        public bool VIP2 { get; set; }

        // VIP Gold
        public bool VIP3 { get; set; }

        public TimeSpan Watched
        {
            get
            {
                return watched;
            }
            set
            {
                watched = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public string GetFollowDateTime(string dtFormat)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string webResponse = wc.DownloadString(string.Format("https://api.twitch.tv/kraken/users/{0}/follows/channels/{1}", UserName, Config.StreamerUsername));
                    JObject json = JObject.Parse(webResponse);

                    // Twitch datetime format: yyyy-mm-ddThh:mm:ss+00:00
                    // Parse it as invariantCulture and roundtrip and ouput shortdate as yyyy-mm-dd ISO format
                    return DateTime.Parse((string)json.GetValue("created_at"), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToString(dtFormat, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                return "Never";
            }
        }

        public bool isFollowing()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString(string.Format("https://api.twitch.tv/kraken/users/{0}/follows/channels/{1}", userName, Config.StreamerUsername));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool isSubscribed()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString(string.Format("https://api.twitch.tv/kraken/channels/{0}/subscriptions/{1}?oauth_token={2}", Config.StreamerUsername, userName, Config.StreamerOAuthKey));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return userName;
        }

        #endregion Public Methods

        #region Private Methods

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion Private Methods

        // DJ TAG to request more songs that others
        // Requested by KiroKnightbow
        //public bool musicDJ { get; set; }
    }
}