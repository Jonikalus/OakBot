using System;

namespace OakBot.Args
{
    public class WinnerChosenEventArgs : EventArgs
    {
        #region Private Fields

        private Viewer winner;

        #endregion Private Fields

        #region Public Constructors

        public WinnerChosenEventArgs(Viewer viewer)
        {
            winner = viewer;
        }

        #endregion Public Constructors

        #region Public Properties

        public Viewer Winner
        {
            get
            {
                return winner;
            }
        }

        #endregion Public Properties
    }
}