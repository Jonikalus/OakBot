using System;

namespace OakBot.Args
{
    public class WinnerChosenEventArgs : EventArgs
    {
        private Viewer winner;

        public WinnerChosenEventArgs(Viewer viewer)
        {
            winner = viewer;
        }

        public Viewer Winner
        {
            get
            {
                return winner;
            }
        }
    }
}