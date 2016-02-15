using RestSharp;

namespace OakBot.Clients
{
    public interface ITwitchClient
    {
        RestRequest GetRequest(string url, Method method);
    }
}