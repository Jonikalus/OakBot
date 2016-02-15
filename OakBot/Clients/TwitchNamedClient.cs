using System;
using RestSharp;
using OakBot.Enums;
using OakBot.Helpers;
using OakBot.Models;

namespace OakBot.Clients
{
    [Obsolete("This class is deprecated, please use TwitchAuthenticatedClient instead.")]
    public class TwitchNamedClient : TwitchAuthenticatedClient, ITwitchClient
    {
        private readonly string username;

        public TwitchNamedClient(string username, string oauth, string clientId) : base(oauth, clientId)
        {
            this.username = username;
        }
    }
}