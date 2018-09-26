using Achievements.Repositories;
using Achievements.Responses;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Achievements.Hubs
{
    /// <summary>
    /// SignalR hub which establishes a connection to the client and stores the user/connection map
    /// </summary>
    public class AchievementsHub : Hub
    {
        private readonly IUserAchievementsRepository<string> _repository;

        public static ConnectionMapping<string> Connections { get; } = new ConnectionMapping<string>();

        public AchievementsHub(IUserAchievementsRepository<string> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adds the user to the connection map
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Connections.Add(GetUserIdFromQueryString(), Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Removes the user from the connection map
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Connections.Remove(GetUserIdFromQueryString(), Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserIdFromQueryString()
        {
            var queryString = Context.GetHttpContext().Request.QueryString;

            if (queryString == null || !queryString.Value.Contains("userId"))
                return null;

            var parsed = HttpUtility.ParseQueryString(queryString.Value);

            return parsed["userId"];
        }
    }
}