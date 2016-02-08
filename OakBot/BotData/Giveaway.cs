using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OakBot
{
    class Giveaway : INotifyPropertyChanged
    {
        #region Fields
        private Timer giveawayTimer;
        public event PropertyChangedEventHandler PropertyChanged;
        private string giveawayName, keyword;
        private int price;
        private bool needsFollow;
        private byte subscriberLuck;
        private TimeSpan responseTime, giveawayTime;
        #endregion Fields

        #region Methods
        private void NotifyPropertyChanged(string info)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void Start()
        {
            giveawayTimer = new Timer();
            giveawayTimer.Interval = GiveawayTime.Seconds + (GiveawayTime.Minutes * 60) + (GiveawayTime.Hours * 60 * 60);
            giveawayTimer.Elapsed += GiveawayTimer_Elapsed;
            giveawayTimer.AutoReset = false;
            giveawayTimer.Start();
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Name of the giveaway
        /// </summary>
        public string GiveawayName {
            get
            {
                return giveawayName;
            }
            set
            {
                giveawayName = value;
                NotifyPropertyChanged("GiveawayName");
            }
        }

        /// <summary>
        /// Keyword to type
        /// </summary>
        public string Keyword {
            get
            {
                return keyword;
            }
            set
            {
                keyword = value;
                NotifyPropertyChanged("Keyword");
            }
        }

        /// <summary>
        /// Price to enter the giveaway
        /// </summary>
        public int Price {
            get
            {
                return price;
            }
            set
            {
                price = value;
                NotifyPropertyChanged("Price");
            }
        }

        /// <summary>
        /// Viewer needs to follow to participate
        /// </summary>
        public bool NeedsFollow {
            get
            {
                return needsFollow;
            }
            set
            {
                needsFollow = value;
                NotifyPropertyChanged("NeedsFollow");
            }
        }

        /// <summary>
        /// Luck for Subscribers (0 is no additional luck, 255 is only subscribers can win)
        /// </summary>
        public byte SubscriberLuck {
            get
            {
                return subscriberLuck;
            }
            set
            {
                subscriberLuck = value;
                NotifyPropertyChanged("SubscriberLuck");
            }
        }

        /// <summary>
        /// Amount of time the viewer has to answer to win the giveaway
        /// </summary>
        public TimeSpan ResponseTime {
            get
            {
                return responseTime;
            }
            set
            {
                responseTime = value;
                NotifyPropertyChanged("ResponseTime");
            }
        }

        /// <summary>
        /// Amount of time a user has to either enter the keyword or type in chat
        /// </summary>
        public TimeSpan GiveawayTime {
            get
            {
                return giveawayTime;
            }
            set
            {
                giveawayTime = value;
                NotifyPropertyChanged("GiveawayTime");
            }
        }

        #endregion Properties

        #region Constructors
        public Giveaway(string name, TimeSpan time)
        {
            GiveawayName = name;
            GiveawayTime = time;

            Keyword = "";
            Price = 0;
            NeedsFollow = false;
            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word)
        {
            GiveawayName = name;
            GiveawayTime = time;
            Keyword = word;
            
            Price = 0;
            NeedsFollow = false;
            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, int cost)
        {
            GiveawayName = name;
            GiveawayTime = time;
            Keyword = word;
            Price = cost;

            NeedsFollow = false;
            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, bool followed)
        {
            GiveawayName = name;
            GiveawayTime = time;
            NeedsFollow = followed;

            Keyword = "";
            Price = 0;
            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, bool followed)
        {
            GiveawayName = name;
            GiveawayTime = time;
            Keyword = word;
            NeedsFollow = followed;

            Price = 0;
            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, bool followed)
        {
            GiveawayName = name;
            GiveawayTime = time;
            Keyword = word;
            NeedsFollow = followed;
            Price = cost;

            SubscriberLuck = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, byte luck)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;

            Keyword = "";
            NeedsFollow = false;
            Price = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, byte luck)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;

            NeedsFollow = false;
            Price = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, byte luck, bool followed)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;
            NeedsFollow = followed;

            Price = 0;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, byte luck)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;
            Price = cost;

            NeedsFollow = false;
            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, byte luck, bool followed)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;
            Price = cost;
            NeedsFollow = followed;

            ResponseTime = new TimeSpan(0, 0, 0);
        }

        public Giveaway(string name, TimeSpan time, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            ResponseTime = response;

            SubscriberLuck = 0;
            Keyword = "";
            Price = 0;
            NeedsFollow = false;
        }

        public Giveaway(string name, TimeSpan time, string word, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            ResponseTime = response;
            Keyword = word;

            SubscriberLuck = 0;
            Price = 0;
            NeedsFollow = false;
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            ResponseTime = response;
            Keyword = word;
            Price = cost;

            SubscriberLuck = 0;
            NeedsFollow = false;
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, byte luck, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            ResponseTime = response;
            Keyword = word;
            Price = cost;
            SubscriberLuck = luck;

            NeedsFollow = false;
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, bool followed, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            ResponseTime = response;
            Keyword = word;
            Price = cost;
            NeedsFollow = followed;

            SubscriberLuck = 0;
        }

        public Giveaway(string name, TimeSpan time, string word, int cost, bool followed, byte luck, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;
            Price = cost;
            NeedsFollow = followed;
            ResponseTime = response;
        }

        #endregion Constructors

        #region Events
        private void GiveawayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            giveawayTimer.Dispose();
            // Notify
        }
        #endregion Events

    }
}
