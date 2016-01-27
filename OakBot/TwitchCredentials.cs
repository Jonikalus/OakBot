namespace OakBot
{
    public class TwitchCredentials
    {
        #region Fields

        private string userName;
        private string oauth;

        #endregion

        #region Constructors

        public TwitchCredentials(string userName, string oauth)
        {
            this.userName = userName;
            this.oauth = oauth;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return userName;
        }

        #endregion

        #region Properties

        public string UserName
        {
            get
            {
                return userName;
            }
        }

        public string OAuth
        {
            get
            {
                return oauth;
            }
        }

        #endregion
    }
}