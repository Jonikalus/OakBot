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

        public void SendChatMessage(string message)
        {
            _chatConnection.IrcClient.WriteLineThrottle(":" + _chatConnection.connectedUser.username +
                "!" + _chatConnection.connectedUser.username + "@" + _chatConnection.connectedUser.username +
                ".tmi.twitch.tv PRIVMSG #" + _joinedChannel.username + " :" + message);
        }

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