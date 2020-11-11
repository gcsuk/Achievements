using System.Threading;
using System.Threading.Tasks;

namespace Achievements.Events
{
    public interface IEventListener
    {
        void RegisterOnMessageHandlerAndReceiveMessages();
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
