using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OakBot.Args
{
    public class ViewerEnteredEventArgs : EventArgs
    {
        private string viewer;

        public string Viewer
        {
            get
            {
                return viewer;
            }
        }

        public ViewerEnteredEventArgs(string viewer)
        {
            this.viewer = viewer;
        }
    }
}
