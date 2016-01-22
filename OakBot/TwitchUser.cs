using System;

namespace OakBot
{
    public class TwitchUser
    {
        #region Fields

        private string _username;
        private string _displayName;
        private string _avatarUri;

        #endregion

        #region Constructors

        public TwitchUser(string username)
        {
            _username = username;
            _displayName = username; //TODO: get this from Twitch
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
                return _displayName;
            }
        }

        #endregion

        // displayName, avatarUri, isSub and isFollowing
        // Should be fetched from Twitch
        // isSub and isFollowing can also be set by the bot on event

        // Channel
        public bool isFollowing { get; set; }
        public bool isSub { get; set; }

        // Points, Hours and Rank
        public long points { get; set; }        // BigInt SQL
        public long raids { get; set; }         // BigInt SQL
        public string rank { get; set; }

        public TimeSpan watchedTimeSpan { get; set; }
        public double hours
        {
            get
            {
                return Math.Round(watchedTimeSpan.TotalHours, 1, MidpointRounding.AwayFromZero);
            }
        }

        // First seen and last message (seen)
        public DateTime dateLastSeen { get; set; }
        public DateTime dateFollow { get; set; }

        // Regular and indicator that streamer/mod removed regular
        public bool isReg { get; set; }
        public bool forcedRegRemove { get; set; }

        // VIP Bronze
        public bool isVIP1 { get; set; }
        public string msgVIP1 { get; set; }
        public DateTime gotVIP1 { get; set; }
        public DateTime expVIP1 { get; set; }

        // VIP Silver
        public bool isVIP2 { get; set; }
        public string msgVIP2 { get; set; }
        public DateTime gotVIP2 { get; set; }
        public DateTime expVIP2 { get; set; }

        // VIP Gold
        public bool isVIP3 { get; set; }
        public string msgVIP3 { get; set; }
        public DateTime gotVIP3 { get; set; }
        public DateTime expVIP3 { get; set; }

        // JOIN and PART messages
        public string msgJoin { get; set; }
        public string msgPart { get; set; }


    }
}