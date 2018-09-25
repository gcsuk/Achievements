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

        /// <summary>
        ///  This is a bit shit but you can sit and monitor the database for new achievements and just tell the client if one appears
        /// </summary>
        [Obsolete("This was added while I was messing about. I left it in just to show a way of doing it without tying SignalR to Service Bus at all.")]
        public async Task MonitorAchievements(string userId)
        {
            while (true) // If you call Stop on the connection while this is running you will get an error
            {
                var newAchievements = await _repository.GetNewForUserId(userId);

                foreach (var achievement in newAchievements)
                {
                    var response = new AchievementUnlockedResponse
                    {
                        Category = achievement.Achievement.Category.Name,
                        Name = achievement.Achievement.Name,
                        Details = achievement.Achievement.Details
                    };

                    await _repository.SetNotNew(userId, achievement.Achievement.Id);

                    foreach (var connectionId in Connections.GetConnections(userId))
                        await Clients.Client(connectionId).SendAsync("Unlocked", JsonConvert.SerializeObject(response));
                }

                Thread.Sleep(60000); // Arbitrary polling period
            }
        }

    }
}