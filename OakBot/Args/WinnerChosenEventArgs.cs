using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot.Args
{
    class WinnerChosenEventArgs : EventArgs
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
