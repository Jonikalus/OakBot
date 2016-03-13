using RestSharp;

namespace OakBot.Clients
{
    public interface ITwitchClient
    {
        #region Public Methods

        RestRequest GetRequest(string url, Method method);

        #endregion Public Methods
    }
}