using System;
using System.ComponentModel;

namespace OakBot
{
    public class TwitchUser : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public TwitchUser(string username)
        {
            _username = username;
            _displayName = username;

            _points = 0;
            _raids = 0;
            _rank = "";

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
            return username;
        }

        #endregion

        #region Properties

        private string _username;
        public string username
        {
            get
            {
                return _username;
            }
        }

        private string _displayName;
        public string displayName
        {
            get
            {
                return _displayName;
            }
        }

        private bool _following;
        public bool following
        {
            get
            {
                return _following;
            }
            set
            {
                if (value != _following)
                {
                    _following = value;
                    NotifyPropertyChanged("following");
                }
            }
        }

        private bool _subscriber;
        public bool subscriber
        {
            get
            {
                return _subscriber;
            }
            set
            {
                if(value != _subscriber)
                {
                    _subscriber = value;
                    NotifyPropertyChanged("subscriber");
                }
            }
        }

        private long _points;       // BigInt SQL
        public long points
        {
            get
            {
                return _points;
            }
            set
            {
                if (value != _points)
                {
                    _points = value;
                    NotifyPropertyChanged("points");
                }
            }
        }

        private long _raids;        // BigInt SQL
        public long raids
        {
            get
            {
                return _raids;
            }
            set
            {
                if (value != _raids)
                {
                    _raids = value;
                    NotifyPropertyChanged("raids");
                }
            }
        }
        
        private string _rank;
        public string rank
        {
            get
            {
                return _rank;
            }
            set
            {
                if (value != _rank)
                {
                    _rank = value;
                    NotifyPropertyChanged("rank");
                }
            }
        }

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

        // DJ TAG to request more songs that others
        // Requested by KiroKnightbow
        //public bool musicDJ { get; set; }

        #endregion
    }
}