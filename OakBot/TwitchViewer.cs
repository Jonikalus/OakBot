using System;
using System.ComponentModel;
using System.Net;
using System.Globalization;
using Newtonsoft.Json.Linq;


namespace OakBot
{
    public class TwitchViewer : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public TwitchViewer(string userName)
        {
            this.userName = userName;

            this.points = 0;
            this.spent = 0;
            this.watched = new TimeSpan(0);
            this.lastSeen = new DateTime(0, DateTimeKind.Utc);
            this.raids = 0;
            this.title = "";
            this.regular = false;
            this.ign = "";
        }

        #endregion

        #region Methods

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
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

        public override string ToString()
        {
            return userName;
        }

        #endregion

        #region Fields and Properties

        // Username of the viewer
        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
        }

        // Points the user has
        private long points;
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

        // Points the user have spent
        private long spent;
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

        // Amount of raids this user did on
        // the users channel (mostly other streamers)
        private long raids;
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
        
        // Title earned by points/hours or bought
        private string title;
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

        // Watched timespan of the viewer
        private TimeSpan watched;
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
        public double Hours
        {
            get
            {
                return Math.Round(watched.TotalHours, 1, MidpointRounding.AwayFromZero);
            }
        }
        public long Minutes
        {
            get
            {
                return Convert.ToInt64(watched.TotalMinutes);
            }
        }

        // Timestamp of last seen
        // This will be updated each background check
        private DateTime lastSeen;
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

        // In-Game-Name field that viewers can set
        private string ign;
        public string IGN
        {
            get
            {
                return ign;
            }
            set
            {
                if(value != ign)
                {
                    title = value;
                    NotifyPropertyChanged("IGN");
                }
            }
        }

        // Regular indicator once point/hours goal has been met
        // Won't be revoked by the bot but can be by the user
        public bool regular { get; set; }
        public bool forcedRegRemove { get; set; }

        // VIP Bronze
        public bool VIP1 { get; set; }
        public DateTime gotVIP1 { get; set; }
        public DateTime expVIP1 { get; set; }

        // VIP Silver
        public bool VIP2 { get; set; }
        public DateTime gotVIP2 { get; set; }
        public DateTime expVIP2 { get; set; }

        // VIP Gold
        public bool VIP3 { get; set; }
        public DateTime gotVIP3 { get; set; }
        public DateTime expVIP3 { get; set; }

        // JOIN and PART messages and comment field
        public string msgJoin { get; set; }
        public string msgPart { get; set; }
        public string comment { get; set; }

        // DJ TAG to request more songs that others
        // Requested by KiroKnightbow
        //public bool musicDJ { get; set; }

        #endregion
    }
}