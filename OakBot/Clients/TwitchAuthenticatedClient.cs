using OakBot.Enums;
using OakBot.Helpers;
using OakBot.Models;
using RestSharp;
using System;

namespace OakBot.Clients
{
    public class TwitchAuthenticatedClient : TwitchReadOnlyClient, ITwitchClient
    {
        #region Private Fields

        private readonly string clientId;
        private readonly string oauth;
        private readonly string username;

        #endregion Private Fields

        #region Public Constructors

        public TwitchAuthenticatedClient(string oauth, string clientId) : base()
        {
            this.oauth = oauth;
            this.clientId = clientId;

            var user = this.GetMyUser();
            if (user == null || String.IsNullOrWhiteSpace(user.Name))
            {
                throw new TwitchException("Couldn't get the user name!");
            }
            this.username = user.Name;
        }

        #endregion Public Constructors

        #region Public Methods

        public TwitchResponse Block(string target)
        {
            var request = GetRequest("users/{user}/blocks/{target}", Method.PUT);
            request.AddUrlSegment("user", username);
            request.AddUrlSegment("target", target);
            var response = restClient.Execute<TwitchResponse>(request);
            return response.Data;
        }

        // has the channel a subscription programm?
        public bool CanSubscribeToChannel(string channel)
        {
            return GetSubscribedChannel(channel).Status != 422;
        }

        public FollowedChannel Follow(string target, bool notifications = false)
        {
            var request = GetRequest("users/{user}/follows/channels/{target}", Method.PUT);
            request.AddUrlSegment("user", username);
            request.AddUrlSegment("target", target);
            request.AddParameter("notifications", notifications);
            var response = restClient.Execute<FollowedChannel>(request);
            return response.Data;
        }

        public TwitchList<Block> GetBlocks()
        {
            var request = GetRequest("users/{user}/blocks", Method.GET);
            request.AddUrlSegment("user", username);
            var response = restClient.Execute<TwitchList<Block>>(request);
            return response.Data;
        }

        public TwitchList<User> GetChannelEditors()
        {
            var request = GetRequest("channels/{channel}/editors", Method.GET);
            request.AddUrlSegment("channel", username);
            var response = restClient.Execute<TwitchList<User>>(request);
            return response.Data;
        }

        public TwitchList<Stream> GetFollowedStreams(PagingInfo pagingInfo = null)
        {
            var request = GetRequest("streams/followed", Method.GET);
            TwitchHelper.AddPaging(request, pagingInfo);
            var response = restClient.Execute<TwitchList<Stream>>(request);
            return response.Data;
        }

        public TwitchList<Video> GetFollowedVideos(PagingInfo pagingInfo = null)
        {
            var request = GetRequest("videos/followed", Method.GET);
            TwitchHelper.AddPaging(request, pagingInfo);
            var response = restClient.Execute<TwitchList<Video>>(request);
            return response.Data;
        }

        public TwitchList<Follower> GetFollowers(PagingInfo pagingInfo = null, SortDirection sortDirection = SortDirection.asc)
        {
            var request = GetRequest("channels/{channel}/follows", Method.GET);
            request.AddUrlSegment("channel", username);
            TwitchHelper.AddPaging(request, pagingInfo);
            request.AddParameter("direction", sortDirection);
            var response = restClient.Execute<TwitchList<Follower>>(request);
            return response.Data;
        }

        public Channel GetMyChannel()
        {
            try
            {
                var request = GetRequest("channel", Method.GET);
                var response = restClient.Execute<Channel>(request);
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new TwitchException("Error fetching channel data", ex);
            }
        }

        public StreamResult GetMyStream()
        {
            return GetStream(username);
        }

        // more details than GetUser(myname)
        public User GetMyUser()
        {
            var request = GetRequest("user", Method.GET);
            var response = restClient.Execute<User>(request);
            return response.Data;
        }

        public RestRequest GetRequest(string url, Method method)
        {
            RestRequest restRequest = new RestRequest(url, method);
            restRequest.AddHeader("Client-ID", clientId);
            restRequest.AddHeader("Authorization", String.Format("OAuth {0}", oauth));
            return restRequest;
        }

        // a channel you subscribed
        public Subscription GetSubscribedChannel(string channel)
        {
            var request = GetRequest("users/{user}/subscriptions/{channel}", Method.GET);
            request.AddUrlSegment("user", username);
            request.AddUrlSegment("channel", channel);
            var response = restClient.Execute<Subscription>(request);
            return response.Data;
        }

        // someone who subscribed your channel
        public Subscription GetSubscriber(string user)
        {
            var request = GetRequest("channels/{channel}/subscriptions/{user}", Method.GET);
            request.AddUrlSegment("channel", username);
            request.AddUrlSegment("user", user);
            var response = restClient.Execute<Subscription>(request);
            return response.Data;
        }

        public TwitchList<Subscription> GetSubscribers(PagingInfo pagingInfo = null, SortDirection sortDirection = SortDirection.asc)
        {
            var request = GetRequest("channels/{channel}/subscriptions", Method.GET);
            request.AddUrlSegment("channel", username);
            TwitchHelper.AddPaging(request, pagingInfo);
            request.AddParameter("direction", sortDirection);
            var response = restClient.Execute<TwitchList<Subscription>>(request);
            return response.Data;
        }

        public bool IsSubscribedToChannel(string channel)
        {
            return GetSubscribedChannel(channel).Status != 404;
        }

        //may return true if request failed
        public bool IsSubscriber(string user)
        {
            int state = GetSubscriber(user).Status;
            return state != 404 && state != 422;
        }

        public User ResetStreamKey()
        {
            var request = GetRequest("channels/{channel}/stream_key", Method.DELETE);
            request.AddUrlSegment("channel", username);
            var response = restClient.Execute<User>(request);
            return response.Data;
        }

        // only for partnered channels
        public Channel SetDelay(long delay)
        {
            return Update(null, null, delay);
        }

        public Channel SetGame(string game)
        {
            return Update(null, game);
        }

        public Channel SetTitle(string title)
        {
            return Update(title);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="length">Length of commercial break in seconds. Default value is 30. Valid values are 30, 60, 90, 120, 150, and 180. You can only trigger a commercial once every 8 minutes.</param>
        /// <returns></returns>
        public TwitchResponse TriggerCommercial(int length)
        {
            var request = GetRequest("channels/{channel}/commercial", Method.POST);
            request.AddUrlSegment("channel", username);
            request.AddParameter("length", length);
            var response = restClient.Execute<TwitchResponse>(request);
            return response.Data;
        }

        public TwitchResponse Unblock(string target)
        {
            var request = GetRequest("users/{user}/blocks/{target}", Method.DELETE);
            request.AddUrlSegment("user", username);
            request.AddUrlSegment("target", target);
            var response = restClient.Execute<TwitchResponse>(request);
            return response.Data;
        }

        public TwitchResponse Unfollow(string target)
        {
            var request = GetRequest("users/{user}/follows/channels/{target}", Method.DELETE);
            request.AddUrlSegment("user", username);
            request.AddUrlSegment("target", target);
            var response = restClient.Execute<TwitchResponse>(request);
            return response.Data;
        }

        public Channel Update(string status = null, string game = null, long? delay = null)
        {
            var request = GetRequest("channels/{channel}", Method.PUT);
            request.AddUrlSegment("channel", username);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { channel = new { status, game, delay } });
            var response = restClient.Execute<Channel>(request);
            return response.Data;
        }

        #endregion Public Methods
    }
}