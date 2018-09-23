using Achievements.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public interface IUserAchievementsRepository<T>
    {
        Task<IEnumerable<UserAchievement<T>>> GetForUserId(T userId);
        Task<T> Add(T userId, int achievementId);
        Task Delete(T userId, int achievementId);
    }
}