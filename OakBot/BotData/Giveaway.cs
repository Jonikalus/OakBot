using OakBot.Args;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool needsFollow, running;
        private byte subscriberLuck;
        private TimeSpan responseTime, giveawayTime;
        private Viewer winner;
        private ObservableCollection<Viewer> entries, winners;
        #endregion Fields

        #region Handlers

        public delegate void WinnerChosenEventHandler(object o, WinnerChosenEventArgs e);
        public event WinnerChosenEventHandler WinnerChosen;

        #endregion Handlers

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
            running = true;
        }

        public void Stop()
        {
            giveawayTimer.Stop();
            giveawayTimer.Dispose();
            running = false;
        }

        public void DrawWinner()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            List<Viewer> list = new List<Viewer>(entries);
            Viewer roll = list[rnd.Next(0, list.Count)];
            while (!MeetsRequirements(roll))
            {
                list.RemoveAll(v => v.UserName == roll.UserName);
                
                roll = list[rnd.Next(0, list.Count)];
            }
            winners.Add(roll);
            winner = roll;
            WinnerChosenEventArgs args = new WinnerChosenEventArgs(winner);
            OnWinnerChosen(args);
        }


        private bool MeetsRequirements(Viewer user)
        {
            if(needsFollow)
            {
                if (!user.isFollowing())
                {
                    return false;
                }
            }
            if(subscriberLuck == 10)
            {
                if (!user.isSubscribed())
                {
                    return false;
                }
            }
            return true;
        }

        protected void OnWinnerChosen(WinnerChosenEventArgs e)
        {
            WinnerChosen(this, e);
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
        /// Luck for Subscribers (0 is no additional luck, 10 is only subscribers can win). Can only be from 0 to 10, if not in this range, it will be 0
        /// </summary>
        public byte SubscriberLuck {
            get
            {
                return subscriberLuck;
            }
            set
            {
                if(subscriberLuck < 10 && subscriberLuck > 0)
                {
                    subscriberLuck = value;
                    NotifyPropertyChanged("SubscriberLuck");
                }else
                {
                    subscriberLuck = 0;
                    NotifyPropertyChanged("SubscriberLuck");
                }
                
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

        public Viewer Winner
        {
            get
            {
                return winner;
            }
        }

        public ObservableCollection<Viewer> Entries
        {
            get
            {
                return entries;
            }
        }

        public bool Running
        {
            get
            {
                return running;
            }
        }
        #endregion Properties

        #region Constructors
        

        public Giveaway(string name, TimeSpan time, string word, int cost, bool followed, byte luck, TimeSpan response)
        {
            GiveawayName = name;
            GiveawayTime = time;
            SubscriberLuck = luck;
            Keyword = word;
            Price = cost;
            NeedsFollow = followed;
            ResponseTime = response;
            entries = new ObservableCollection<Viewer>();
            winners = new ObservableCollection<Viewer>();
        }

        #endregion Constructors

        #region Events
        private void GiveawayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            giveawayTimer.Dispose();
            running = false;
        }
        #endregion Events

    }

}
