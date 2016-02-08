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
        private string game;
        private bool displayGame;
        private DateTime date;

        private string addedBy;
        private DateTime lastDisplayed;

        #endregion

        #region Constructor

        /// <summary>
        /// Use to add new quote to the bot
        /// </summary>
        /// <param name="id">Unique ID of the quote, verfify before adding</param>
        /// <param name="quote">The quote itself</param>
        /// <param name="quoter">The person saying the quote</param>
        /// <param name="game">The game that the streamer was playing when the quote was made</param>
        /// <param name="displayName">Displaying of the game</param>
        /// <param name="addedBy">Name of the viewer that added the quote</param>
        public Quote (long id, string quote, string quoter, string game, bool displayGame, string addedBy)
        {
            this.id = id;
            this.quote = quote;
            this.quoter = quoter;
            this.game = game;
            this.displayGame = displayGame;
            this.addedBy = addedBy;
            this.date = DateTime.UtcNow;
            this.lastDisplayed = DateTime.MinValue;
        }

        /// <summary>
        /// Use to load in existing quotes from a database
        /// </summary>
        /// <param name="id">Unique ID of the quote, verfify before adding</param>
        /// <param name="quote">The quote itself</param>
        /// <param name="quoter">The person saying the quote</param>
        /// <param name="game">The game that the streamer was playing when the quote was made</param>
        /// <param name="displayName">Displaying of the game</param>
        /// <param name="addedBy">Name of the viewer that added the quote</param>
        /// <param name="date">DateTime of the quote was made</param>
        /// <param name="lastDisplayed">DateTime of when the quote was last displayed (in chat)</param>
        public Quote(long id, string quote, string quoter, string game, bool displayGame, string addedBy, DateTime date, DateTime lastDisplayed)
        {
            this.id = id;
            this.quote = quote;
            this.quoter = quoter;
            this.game = game;
            this.displayGame = displayGame;
            this.addedBy = addedBy;
            this.date = date;
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

        // Game is editable, INotify required
        public string Game
        {
            get
            {
                if (displayGame)
                {
                    return game;
                }
                else
                {
                    return "";
                }
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

        // GameString is depended on Game and displayGame
        // Returns an empty string if displayGame is false
        public string GameString
        {
            get
            {
                if (displayGame)
                {
                    return game;
                }
                else
                {
                    return "";
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
                return Date.ToString("yyyy-MM-dd hh:mm");
            }
        }

        // AddedBy cannot be changed by the user
        public string AddedBy
        {
            get
            {
                return addedBy;
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
                    return lastDisplayed.ToString("yyyy-MM-dd hh:mm");
                }
            }
        }

        #endregion

    }
}
