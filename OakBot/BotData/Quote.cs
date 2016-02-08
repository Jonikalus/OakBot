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
        private DateTime date;

        #endregion

        #region Constructor

        public Quote (long id, string quote, string quoter, string game)
        {
            this.id = id;
            this.quote = quote;
            this.quoter = quoter;
            this.game = game;
            this.date = DateTime.UtcNow;
        }

        public Quote(long id, string quote, string quoter, string game, DateTime date)
        {
            this.id = id;
            this.quote = quote;
            this.quoter = quoter;
            this.game = game;
            this.date = date;
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

        #endregion

        #region Properties

        // Id cannot be changed by the user.
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
        #endregion

    }
}
