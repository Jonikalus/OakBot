namespace OakBot
{
    public class TwitchConnection
    {
        #region Public Fields

        public readonly string clientId;
        public readonly string oAuthToken;
        public readonly string username;

        #endregion Public Fields

        #region Public Constructors

        public TwitchConnection(string username, string clientId, string oAuthToken)
        {
            this.username = username;
            this.clientId = clientId;
            this.oAuthToken = oAuthToken;
        }

        #endregion Public Constructors
    }
}