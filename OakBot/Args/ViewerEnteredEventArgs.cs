using System;

namespace OakBot.Args
{
    public class ViewerEnteredEventArgs : EventArgs
    {
        #region Private Fields

        private string viewer;

        #endregion Private Fields

        #region Public Constructors

        public ViewerEnteredEventArgs(string viewer)
        {
            this.viewer = viewer;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Viewer
        {
            get
            {
                return viewer;
            }
        }

        #endregion Public Properties
    }
}