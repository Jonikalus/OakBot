using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot
{
    public class Quote : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        private long id;
        private string quote;
        private string quoter;
        private DateTime date;
        private bool displayDate;
        private string game;
        private bool displayGame;
        
        private DateTime lastDisplayed;

        #endregion

        #region Constructor

        /// <summary>
        /// Use to add new quote to the bot by command
        /// </summary>
        /// <param name="quote">The quote itself</param>
        /// <param name="quoter">The person saying the quote</param>
        /// <param name="game">The game that the streamer was playing when the quote was made</param>
        public Quote (string quote, string quoter, string game)
        {
            this.quote = quote;
            this.quoter = quoter;
            this.game = game;

            this.id = MainWindow.colQuotes.Count()-1;
            this.displayDate = true;
            this.displayGame = true;
            this.date = DateTime.UtcNow;
            this.lastDisplayed = DateTime.MinValue;
        }

        /// <summary>
        /// Use to load in existing quotes from a database or from UI
        /// </summary>
        /// <param name="id">Unique ID of the quote, verfify before adding</param>
        /// <param name="quote">The quote itself</param>
        /// <param name="quoter">The person saying the quote</param>
        /// <param name="date">DateTime of the quote was made</param>
        /// <param name="displayDate">Displaying of the quote date</param>
        /// <param name="game">The game that the streamer was playing when the quote was made</param>
        /// <param name="displayGame">Displaying of the game</param>
        /// <param name="addedBy">Name of the viewer that added the quote</param>
        /// <param name="lastDisplayed">DateTime of when the quote was last displayed (in chat)</param>
        public Quote (long id, string quote, string quoter, DateTime date, bool displayDate, string game,
            bool displayGame, DateTime lastDisplayed)
        {
            this.id = id;
            this.quote = quote;
            this.quoter = quoter;
            this.date = date;
            this.displayDate = displayDate;
            this.game = game;
            this.displayGame = displayGame;
            this.lastDisplayed = lastDisplayed;
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

        public void SetLastDisplayed()
        {
            lastDisplayed = DateTime.UtcNow;
        }

        #endregion

        #region Properties

        // Id cannot be changed by the user
        public long Id
        {
            get
            {
                return id;
            }
        }

        // Quote is editable, INotify required
        public string QuoteString
        {
            get
            {
                return quote;
            }
            set
            {
                if (value != quote)
                {
                    quote = value;
                    NotifyPropertyChanged("QuoteString");
                }
            }
        }

        // Quoter is editable, INotify required
        public string Quoter
        {
            get
            {
                return quoter;
            }
            set
            {
                if (value != quoter)
                {
                    quoter = value;
                    NotifyPropertyChanged("Quoter");
                }
            }
        }

        // Date is editable, INotify required
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (value != date)
                {
                    date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        // DateString is depended on Date
        public string DateString
        {
            get
            {
                return Date.ToString("yyyy-MM-dd");
            }
        }

        // DisplayDate is editable, INotify required
        public bool DisplayDate
        {
            get
            {
                return displayDate;
            }
            set
            {
                if(value != displayDate)
                {
                    displayDate = value;
                    NotifyPropertyChanged("DisplayDate");
                }
            }
        }

        public string DisplayDateString
        {
            get
            {
                if (displayDate)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }

        // Game is editable, INotify required
        public string Game
        {
            get
            {
                return game;
            }
            set
            {
                if (value != game)
                {
                    game = value;
                    NotifyPropertyChanged("Game");
                }
            }
        }

        // DisplayGame is editable, INotify required
        public bool DisplayGame
        {
            get
            {
                return displayGame;
            }
            set
            {
                if (value != displayGame)
                {
                    displayGame = value;
                    NotifyPropertyChanged("DisplayGame");
                }
            }
        }

        public string DisplayGameString
        {
            get
            {
                if (displayGame)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }


        // LastDisplayed cannot bt changed by the user
        public DateTime LastDisplayed
        {
            get
            {
                return lastDisplayed;
            }
        }
        
        // LastDisplayedString is depended on LastDisplayed
        // Returns Never when lastDisplayed is not set
        public string LastDisplayedString
        {
            get
            {
                if (lastDisplayed == DateTime.MinValue)
                {
                    return "Never";
                }
                else
                {
                    return lastDisplayed.ToString("yyyy-MM-dd");
                }
            }
        }

        #endregion

    }
}
