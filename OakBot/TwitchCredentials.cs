namespace OakBot
{
    class TwitchCredentials
    {
        #region Fields

        private TwitchUser _user;
        private string _oauth;

        #endregion

        #region Constructors

        public TwitchCredentials(TwitchUser user, string oauth)
        {
            _user = user;
            _oauth = oauth;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return _user.username;
        }

        #endregion

        #region Properties

        public string username
        {
            get
            {
                return _user.username;
            }
        }

        public string oauth
        {
            get
            {
                return _oauth;
            }
        }

        #endregion
    }
}