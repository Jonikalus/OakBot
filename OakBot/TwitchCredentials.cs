namespace OakBot
{
    public class TwitchCredentials
    {
        #region Fields

        private string _username;
        private string _oauth;

        #endregion

        #region Constructors

        public TwitchCredentials(string username, string oauth)
        {
            _username = username;
            _oauth = oauth;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return _username;
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