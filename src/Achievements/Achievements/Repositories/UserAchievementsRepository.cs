using Achievements.Models;
using Dapper;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
                    "       Achievements.Id AS AchievementId, Achievements.*, " +
                    "       Categories.Id AS CategoryId, Categories.* " +
                    "FROM   UserAchievements  " +
                    "INNER JOIN Achievements ON Achievements.Id = UserAchievements.AchievementId " +
                    "INNER JOIN Categories ON Categories.Id = Achievements.CategoryId " +
                    "WHERE  UserAchievements.UserId = @userId";

                dbConnection.Open();

                var userAchievements = await dbConnection.QueryAsync<UserAchievement<string>, Achievement, Category, UserAchievement<string>>(
                    sql,
                    (userAchievement, achievement, category) =>
                    {
                        userAchievement.Achievement = achievement;
                        userAchievement.Achievement.Category = category;

                        return userAchievement;
                    },
                    splitOn: "AchievementId, CategoryId",
                    param: new { userId }
                );

                return userAchievements;
            }
        }

        public async Task<IEnumerable<UserAchievement<string>>> GetNewForUserId(string userId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string sql =
                    "SELECT UserAchievements.UserId, UserAchievements.DateUnlocked, UserAchievements.IsNew, " +
                    "       Achievements.Id AS AchievementId, Achievements.*, " +
                    "       Categories.Id AS CategoryId, Categories.* " +
                    "FROM   UserAchievements  " +
                    "INNER JOIN Achievements ON Achievements.Id = UserAchievements.AchievementId " +
                    "INNER JOIN Categories ON Categories.Id = Achievements.CategoryId " +
                    "WHERE  UserAchievements.UserId = @userId" +
                    "       AND IsNew = 1";

                dbConnection.Open();

                var userAchievements = await dbConnection.QueryAsync<UserAchievement<string>, Achievement, Category, UserAchievement<string>>(
                    sql,
                    (userAchievement, achievement, category) =>
                    {
                        userAchievement.Achievement = achievement;
                        userAchievement.Achievement.Category = category;

                        return userAchievement;
                    },
                    splitOn: "AchievementId, CategoryId",
                    param: new { userId }
                );

                return userAchievements;
            }
        }

        public async Task SetNotNew(string userId, int achievementId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "UPDATE UserAchievements SET IsNew = 0 WHERE UserId = @userId AND AchievementId = @achievementId";

                dbConnection.Open();

                await dbConnection.ExecuteAsync(query, new { userId, achievementId });
            }
        }
    }
}