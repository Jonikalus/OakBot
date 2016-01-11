namespace OakBot
{
    class TwitchChatChannel
    {
        private TwitchChatConnection _chatConnection;
        private TwitchUser _joinedChannel;

        #region Constructors

        public TwitchChatChannel(TwitchChatConnection chatConnection,
            TwitchUser userChannel)
        {
            _chatConnection = chatConnection;
            _joinedChannel = userChannel;

            chatConnection.JoinChannel(this);
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public string name
        {
            get
            {
                return _joinedChannel.username;
            }
        }

        #endregion
    }
}