using Achievements.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public class UserAchievementsRepository : IRepository<UserAchievement<string>, string>
    {
        private readonly string _connectionString;

        public UserAchievementsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> Add(UserAchievement<string> item)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "INSERT INTO UserAchievements " +
                                     "(UserId, AchievementId) " +
                                     "VALUES " +
                                     "(@UserId, @AchievementId)";

                dbConnection.Open();

                return (await dbConnection.ExecuteAsync(query, item)).ToString();
            }
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserAchievement<string>>> GetAll()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
                return await dbConnection.QueryAsync<UserAchievement<string>>("SELECT * FROM UserAchievements");
        }

        public async Task<UserAchievement<string>> GetByID(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(UserAchievement<string> item)
        {
            throw new NotImplementedException();
        }
    }
}
