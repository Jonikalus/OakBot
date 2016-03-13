using System;

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