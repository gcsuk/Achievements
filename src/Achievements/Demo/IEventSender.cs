using System.Threading.Tasks;
using Achievements.Events;

namespace Achievements.Demo
{
    public interface IEventSender
    {
        Task Send(IAchievementEvent @event);
        Task CloseAsync();
    }
}
