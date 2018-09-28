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

    }
}