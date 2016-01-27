using System;
using System.ComponentModel;

namespace OakBot
{
    public class TwitchViewer : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public TwitchViewer(string username)
        {
            this.userName = username;
            displayName = username;

            points = 0;
            raids = 0;
            rank = "";

            following = false;
            subscriber = false;

            watchedTimeSpan = new TimeSpan(0);
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

        public override string ToString()
        {
            return userName;
        }

        #endregion

        #region Fields and Properties

        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
        }

        private string displayName;
        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        private bool following;
        public bool Following
        {
            get
            {
                return following;
            }
            set
            {
                if (value != following)
                {
                    following = value;
                    NotifyPropertyChanged("Following");
                }
            }
        }

        private bool subscriber;
        public bool Subscriber
        {
            get
            {
                return subscriber;
            }
            set
            {
                if(value != subscriber)
                {
                    subscriber = value;
                    NotifyPropertyChanged("Subscriber");
                }
            }
        }

        private long points;       // BigInt SQL
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

        private long raids;        // BigInt SQL
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
        
        private string rank;
        public string Rank
        {
            get
            {
                return rank;
            }
            set
            {
                if (value != rank)
                {
                    rank = value;
                    NotifyPropertyChanged("Rank");
                }
            }
        }

        // Watched timespan
        public TimeSpan watchedTimeSpan { get; set; }
        public double Hours
        {
            get
            {
                return Math.Round(watchedTimeSpan.TotalHours, 1, MidpointRounding.AwayFromZero);
            }
        }
        public long Minutes // BigInt SQL
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

        // DJ TAG to request more songs that others
        // Requested by KiroKnightbow
        //public bool musicDJ { get; set; }

        #endregion
    }
}