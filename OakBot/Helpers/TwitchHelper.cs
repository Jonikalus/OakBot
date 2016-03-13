using RestSharp;

namespace OakBot.Helpers
{
    public class TwitchHelper
    {
        #region Public Fields

        public const string twitchAcceptHeader = "application/vnd.twitchtv.v3+json";
        public const string twitchApiUrl = "https://api.twitch.tv/kraken";

        #endregion Public Fields

        #region Public Methods

        public static void AddPaging(IRestRequest request, PagingInfo pagingInfo)
        {
            if (pagingInfo == null) return;
            request.AddParameter("limit", pagingInfo.PageSize);
            request.AddParameter("offset", pagingInfo.Skip);
        }

        #endregion Public Methods
    }
}