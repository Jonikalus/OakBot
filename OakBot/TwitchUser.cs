using System;

namespace OakBot
{
    public class TwitchUser
    {
        #region Fields

        private string _username;
        //private string _displayName;
        //private string _avatarUri;

        #endregion

        #region Constructors

        public TwitchUser(string username)
        {
            _username = username;

            points = 0;
            raids = 0;
            rank = "";

            following = false;
            subscriber = false;

            watchedTimeSpan = new TimeSpan(0);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return username;
        }

        #endregion

        #region Properties

        public string username
        {
            get
            {
                return _username;
            }
        }

        public string displayName
        {
            get
            {
                return _username;
            }
        }

        #endregion

        // displayName, avatarUri, isSub and isFollowing
        // Should be fetched from Twitch
        // isSub and isFollowing can also be set by the bot on event

        // Channel
        public bool following { get; set; }     // Bool SQL
        public bool subscriber { get; set; }    // Bool SQL

        // Points, raids, Hours and Rankname
        public long points { get; set; }        // BigInt SQL
        public long raids { get; set; }         // BigInt SQL
        public string rank { get; set; }        // Text SQL

        // Watched timespan
        public TimeSpan watchedTimeSpan { get; set; }
        public double hours
        {
            get
            {
                return Math.Round(watchedTimeSpan.TotalHours, 1, MidpointRounding.AwayFromZero);
            }
        }
        public long minutes // BigInt SQL
        {
            get
            {
                return Convert.ToInt64(watchedTimeSpan.TotalMinutes);
            }
        }

        // First seen and last message (seen)
        public DateTime dateLastSeen { get; set; }
        public DateTime dateFollow { get; set; }

        // Regular and indicator that streamer/mod removed regular
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


    }
}