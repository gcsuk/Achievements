using Achievements.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public class AchievementsRepository : IRepository<Achievement, int>
    {
        private readonly string _connectionString;

        public AchievementsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> Add(Achievement item)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "INSERT INTO Achievements " +
                                     "(CategoryId, [Name], Details, IsSecret) " +
                                     "VALUES " +
                                     "(@CategoryId, @Name, @Details, @IsSecret); " +
                                     "SELECT @@IDENTITY AS Id";

                dbConnection.Open();

                return (await dbConnection.QueryAsync<int>(query, item)).Single();
            }
        }

        public async Task Delete(int id)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
                await dbConnection.ExecuteAsync("DELETE FROM Achievements WHERE Id = @id", new { id });
        }

        public async Task<IEnumerable<Achievement>> GetAll()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
                return await dbConnection.QueryAsync<Achievement>("SELECT * FROM Achievements");
        }

        public async Task<Achievement> GetByID(int id)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "SELECT * FROM Achievements WHERE Id = @Id";
                dbConnection.Open();
                var settings = await dbConnection.QueryAsync<Achievement>(query, new { id });
                return settings.FirstOrDefault();
            }
        }

        public async Task Update(Achievement item)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                const string query = "UPDATE Achievements " +
                                     "SET    CategoryId = @CategoryId, " +
                                     "       [Name] = @Name, " +
                                     "       Details = @Details, " +
                                     "       IsSecret = @IsSecret " +
                                     "WHERE  Id = @Id";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(query, item);
            }
        }
    }
}
