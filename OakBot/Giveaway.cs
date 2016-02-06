using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OakBot
{
    class Giveaway
    {
        private Timer giveawayTimer;

        #region Properties
        /// <summary>
        /// Name of the giveaway
        /// </summary>
        public string GiveawayName { get; set; }

        /// <summary>
        /// Keyword to type
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Price to enter the giveaway
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Viewer needs to follow to participate
        /// </summary>
        public bool NeedsFollow { get; set; }

        /// <summary>
        /// Luck for Subscribers (0 is no additional luck, 255 is only subscribers can win)
        /// </summary>
        public byte SubscriberLuck { get; set; }

        /// <summary>
        /// Amount of time the viewer has to answer to win the giveaway
        /// </summary>
        public TimeSpan ResponseTime { get; set; }

        /// <summary>
        /// Amount of time a user has to either enter the keyword or type in chat
        /// </summary>
        public TimeSpan GiveawayTime { get; set; }

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

        public void Start()
        {
            giveawayTimer = new Timer();
            giveawayTimer.Interval = GiveawayTime.Seconds + (GiveawayTime.Minutes * 60) + (GiveawayTime.Hours * 60 * 60);
            giveawayTimer.Elapsed += GiveawayTimer_Elapsed;
            giveawayTimer.AutoReset = false;
            giveawayTimer.Start();
        }

        private void GiveawayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            giveawayTimer.Dispose();
            // Notify
        }

       
    }
}
