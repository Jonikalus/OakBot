namespace OakBot
{
    public class TwitchUser
    {
        #region Fields

        private string _username;
        private string _displayName;
        //private string _uri;

        #endregion
        
        #region Constructors

        public TwitchUser(string username)
        {
            _username = username;
            _displayName = username; //TODO: get this from Twitch
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

        public string displayName
        {
            get
            {
                return _displayName;
            }
        }

        #endregion
    }
}