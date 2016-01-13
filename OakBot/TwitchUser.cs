namespace OakBot
{
    public class TwitchUser
    {
        #region Fields

        private string _username;
        //private string _displayname;
        //private string _uri;

        #endregion
        
        #region Constructors

        public TwitchUser(string username)
        {
            _username = username;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return username;
        }

        #endregion

        #region Properties

        public string username
        {
            get
            {
                return _username;
            }
        }

        #endregion
    }
}