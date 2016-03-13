namespace OakBot
{
    public class TwitchCredentials
    {
        #region Private Fields

        private string oauth;
        private string userName;

        #endregion Private Fields

        #region Public Constructors

        public TwitchCredentials(string userName, string oauth)
        {
            this.userName = userName;
            this.oauth = oauth;
        }

        #endregion Public Constructors

        #region Public Properties

        public string OAuth
        {
            get
            {
                return oauth;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return userName;
        }

        #endregion Public Methods
    }
}