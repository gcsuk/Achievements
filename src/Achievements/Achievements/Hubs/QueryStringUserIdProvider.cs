using System;
using Microsoft.AspNetCore.SignalR;

namespace Achievements.Hubs
{
	public class QueryStringUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext().Request.Query["userId"];
        }
    }
}
