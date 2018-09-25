using Achievements.Events;
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
    public class AchievementsHub : Hub
    {
        private readonly IUserAchievementsRepository<string> _repository;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public AchievementsHub(IUserAchievementsRepository<string> repository)
        {
            _repository = repository;
        }

        public async Task MonitorAchievements(string userId)
        {
            // TODO: Sort this out
            // This is a shit way to do it, but it will work until I can figure out how to get the service bus event handler to trigger NotifyAchievement
            while (true)
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

                    foreach (var connectionId in _connections.GetConnections(userId))
                        await Clients.Client(connectionId).SendAsync("Unlocked", JsonConvert.SerializeObject(response));
                }

                Thread.Sleep(10000);
            }
        }

        public async Task NotifyAchievement(string userId, AchievementUnlockedEvent @event)
        {
            foreach (var connectionId in _connections.GetConnections(userId))
            {
                await Clients.Client(connectionId).SendAsync("Unlocked", JsonConvert.SerializeObject(@event));
            }
        }

        public override async Task OnConnectedAsync()
        {
            _connections.Add(GetUserIdFromQueryString(), Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connections.Remove(GetUserIdFromQueryString(), Context.ConnectionId);

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
