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

            // Default values, can be changed in the edit menu
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
        public Quote (string quote, string quoter, DateTime date, bool displayDate, string game,
            bool displayGame, DateTime lastDisplayed)
        {
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

        /// <summary>
        /// Private method to notify INotify on change
        /// </summary>
        /// <param name="property"></param>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Set Last displayed to current UTC timestamp
        /// </summary>
        public void SetLastDisplayed()
        {
            lastDisplayed = DateTime.UtcNow;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Return the Index of the quote in the colQuotes
        /// </summary>
        public long Id
        {
            get
            {
                return MainWindow.colQuotes.IndexOf(this);
            }
        }

        /// <summary>
        /// Quote itself, has INotify
        /// </summary>
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

        /// <summary>
        /// Quoter of the quote, has INotify
        /// </summary>
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

        /// <summary>
        /// The date the quote was made, has INotify
        /// </summary>
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

        /// <summary>
        /// YYYY-MM-DD output of the Date the quote was made.
        /// </summary>
        public string DateString
        {
            get
            {
                return Date.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Display the Date, has INotify
        /// </summary>
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

        /// <summary>
        /// Returns "Yes" or "No" for display date
        /// </summary>
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

        /// <summary>
        /// The game the streamer was playing during the quote, has INotify
        /// </summary>
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

        /// <summary>
        /// Display Game, has INotify
        /// </summary>
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

        /// <summary>
        /// Returns "Yes" or "No" for display game 
        /// </summary>
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

        /// <summary>
        /// Date of the last display of the quote, has INotify
        /// </summary>
        public DateTime LastDisplayed
        {
            get
            {
                return lastDisplayed;
            }
            set
            {
                if (value != lastDisplayed)
                {
                    lastDisplayed = value;
                    NotifyPropertyChanged("LastDisplayed");
                }
            }
        }

        /// <summary>
        /// YYYY-MM-DD output of the Date the quote was last displayed
        /// </summary>
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
