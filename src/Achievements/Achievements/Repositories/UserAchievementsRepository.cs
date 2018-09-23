using Achievements.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public class UserAchievementsRepository : IUserAchievementsRepository<string>
    {
        private readonly string _connectionString;

        public UserAchievementsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> Add(string userId, int achievementId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "INSERT INTO UserAchievements " +
                                     "(UserId, AchievementId) " +
                                     "VALUES " +
                                     "(@userId, @achievementId)";

                dbConnection.Open();

                return (await dbConnection.ExecuteAsync(query, new { userId, achievementId })).ToString();
            }
        }

        public async Task Delete(string userId, int achievementId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "DELETE FROM UserAchievements WHERE UserId = @userId AND AchievementId = @achievementId";

                dbConnection.Open();

                await dbConnection.ExecuteAsync(query, new { userId, achievementId });
            }
        }

        public async Task<IEnumerable<UserAchievement<string>>> GetAll()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string sql =
                    "SELECT UserAchievements.UserId, UserAchievements.DateUnlocked, UserAchievements.IsNew, " +
                    "       Achievements.Id AS AchievementId, Achievements.* " +
                    "FROM   UserAchievements  " +
                    "INNER JOIN Achievements ON Achievements.Id = UserAchievements.AchievementId ";

                dbConnection.Open();

                var achievementDictionary = new Dictionary<int, UserAchievement<string>>();

                var userAchievements = await dbConnection.QueryAsync<UserAchievement<string>, Achievement, UserAchievement<string>>(
                    sql,
                    (userAchievement, achievement) =>
                    {
                        userAchievement.Achievement = achievement;

                        return userAchievement;
                    },
                    splitOn: "AchievementId"
                );

                return userAchievements;
            }
        }

        public async Task<IEnumerable<UserAchievement<string>>> GetForUserId(string userId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string sql =
                    "SELECT UserAchievements.UserId, UserAchievements.DateUnlocked, UserAchievements.IsNew, " +
                    "       Achievements.Id AS AchievementId, Achievements.* " +
                    "FROM   UserAchievements  " +
                    "INNER JOIN Achievements ON Achievements.Id = UserAchievements.AchievementId " +
                    "WHERE  UserAchievements.UserId = @userId";

                dbConnection.Open();

                var achievementDictionary = new Dictionary<int, UserAchievement<string>>();

                var userAchievements = await dbConnection.QueryAsync<UserAchievement<string>, Achievement, UserAchievement<string>>(
                    sql,
                    (userAchievement, achievement) =>
                    {
                        userAchievement.Achievement = achievement;

                        return userAchievement;
                    },
                    splitOn: "AchievementId",
                    param: new { userId }
                );

                return userAchievements;
            }
        }
    }
}