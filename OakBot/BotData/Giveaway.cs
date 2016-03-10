using OakBot.Args;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading;
using System.Timers;

namespace OakBot
{
    public class Giveaway : INotifyPropertyChanged
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
        private ObservableCollection<string> entries, winners;
        #endregion Fields

        #region Handlers

        public delegate void WinnerChosenEventHandler(object o, WinnerChosenEventArgs e);
        public event WinnerChosenEventHandler WinnerChosen;

        public delegate void ViewerEnteredEventHandler(object o, ViewerEnteredEventArgs e);
        public event ViewerEnteredEventHandler ViewerEntered;

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
            //TimerCallback cb = delegate (object o)
            //{
            //    running = false;
            //    MainWindow.instance.botChatConnection.SendChatMessage(string.Format("{0} giveaway ended! Winner will be drawn by the streamer!", giveawayName));
            //};
            //giveawayTimer = new Timer(cb, new ManualResetEvent(true), new TimeSpan(0), giveawayTime);
            running = true;
            MainWindow.instance.botChatConnection.ChatMessageReceived += BotChatConnection_ChatMessageReceived;
            MainWindow.instance.botChatConnection.SendChatMessage(string.Format("{0} giveaway has started! Type {1} in chat to enter! Following is {2} Duration: {3}h {4}m {5}s", giveawayName, keyword, (needsFollow ? "needed." : "not needed."), giveawayTime.Hours, giveawayTime.Minutes, giveawayTime.Seconds));
        }

        private void BotChatConnection_ChatMessageReceived(object o, ChatMessageReceivedEventArgs e)
        {
            if(keyword != "")
            {
                if(e.Message.Message == keyword)
                {
                    if (!entries.Contains(e.Message.Author))
                    {
                        entries.Add(e.Message.Author);
                        ViewerEntered(this, new ViewerEnteredEventArgs(e.Message.Author));
                        MainWindow.instance.botChatConnection.SendChatMessage(string.Format("{0}, you've entered the {1} giveaway!", e.Message.Author, giveawayName));
                    }
                }
            }else
            {
                if (!entries.Contains(e.Message.Author))
                {
                    entries.Add(e.Message.Author);
                    ViewerEntered(this, new ViewerEnteredEventArgs(e.Message.Author));
                }
            }
        }

        public void Stop()
        {
            giveawayTimer.Dispose();
            running = false;
            MainWindow.instance.botChatConnection.SendChatMessage(string.Format("{0} giveaway ended! Winner will be drawn by the streamer!"));
        }

        public void DrawWinner()
        {
            // Create a random instance with timebased seed
            Random rnd = new Random((int)DateTime.Now.Ticks);

            // Create a copy of the current entry list to work with
            // This also prevents people to enter when rolling is happening
            List<string> workList = new List<string>(entries);

            // Manupilate workList to add subscriber luck

            /*
                    // Not really needed as the forloop will limit this but to increase performance
                    // it won't load subscriber list if subluck is set to 1 (off)
                    if (SubscriberLuck > 1)
                    {
                        List<string> subList = new List<string>();

                        // TODO: Fetch complete subscriber list from Twitch API and add to subList

                        // Create another list with Intersect restult as it is going to enumerate
                        // over this while editing workList to prevent modified execeptions.
                        List<string> crossCheck = new List<string>(workList.Intersect(subList));
                        foreach (string subEntry in crossCheck)
                        {
                            for (int i = 1; i < subscriberLuck; i++)
                            {
                                workList.Insert(rnd.Next(0, workList.Count), subEntry);
                            }
                        }
                    }
            */


            // Roll initial winner and get the Viewer object
            // No need to verify if user exist as it SHOULD exist
            int index = rnd.Next(0, workList.Count);
            Viewer rolledViewer = MainWindow.colDatabase.FirstOrDefault(x => 
                x.UserName == workList[index]);

            // Verify if the winner is eligable, if not remove from
            // the workList and reroll a winner without user interaction
            while (!MeetsRequirements(rolledViewer) || rolledViewer == null)
            {
                try
                {
                    // Remove failed winner from workList
                    workList.RemoveAll(x => x == rolledViewer.UserName);

                    // Reroll winner
                    index = rnd.Next(0, workList.Count);
                    rolledViewer = MainWindow.colDatabase.FirstOrDefault(x =>
                        x.UserName == workList[index]);
                }
                catch (Exception)
                {
                    
                }
            }

            // We finally have a winner!
            winners.Add(rolledViewer.UserName);
            winner = rolledViewer;

            WinnerChosenEventArgs args = new WinnerChosenEventArgs(rolledViewer);
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

        public ObservableCollection<string> Entries
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
            giveawayName = name;
            giveawayTime = time;
            subscriberLuck = luck;
            keyword = word;
            price = cost;
            needsFollow = followed;
            responseTime = response;
            entries = new ObservableCollection<string>();
            winners = new ObservableCollection<string>();

            giveawayTimer = new Timer();
            giveawayTimer.Interval = giveawayTime.TotalMilliseconds;
            giveawayTimer.Enabled = true;
            giveawayTimer.Elapsed += GiveawayTimer_Elapsed;
            giveawayTimer.AutoReset = false;


        }

        #endregion Constructors

        #region Events
        private void GiveawayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            running = false;
            MainWindow.instance.botChatConnection.SendChatMessage(string.Format("{0} giveaway ended! Winner will be drawn by the streamer!", giveawayName));
        }
        #endregion Events

    }

}
