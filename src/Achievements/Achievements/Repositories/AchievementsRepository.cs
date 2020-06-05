using Achievements.Models;
using Dapper;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
            await using var dbConnection = new SqlConnection(_connectionString);

            const string query = "INSERT INTO Achievements " +
                                 "(CategoryId, [Name], Details, IsSecret) " +
                                 "VALUES " +
                                 "(@CategoryId, @Name, @Details, @IsSecret); " +
                                 "SELECT @@IDENTITY AS Id";

            dbConnection.Open();

            return (await dbConnection.QueryAsync<int>(query, new { CategoryId = item.Category.Id, item.Name, item.Details, item.IsSecret })).Single();
        }

        public async Task Delete(int id)
        {
            await using var dbConnection = new SqlConnection(_connectionString);

            await dbConnection.ExecuteAsync("DELETE FROM Achievements WHERE Id = @id", new { id });
        }

        public async Task<IEnumerable<Achievement>> GetAll()
        {
            await using var dbConnection = new SqlConnection(_connectionString);

            const string sql =
                "SELECT Achievements.*, " +
                "       Categories.Id AS CategoryId, Categories.* " +
                "FROM   Achievements " +
                "INNER JOIN Categories ON Categories.Id = Achievements.CategoryId ";

            dbConnection.Open();

            var achievements = await dbConnection.QueryAsync<Achievement, Category, Achievement>(
                sql,
                (achievement, category) =>
                {
                    achievement.Category = category;

                    return achievement;
                },
                splitOn: "CategoryId"
            );

            return achievements;
        }

        public async Task<Achievement> GetByID(int id)
        {
            await using var dbConnection = new SqlConnection(_connectionString);

            const string sql =
                "SELECT Achievements.*, " +
                "       Categories.Id AS CategoryId, Categories.* " +
                "FROM   Achievements " +
                "INNER JOIN Categories ON Categories.Id = Achievements.CategoryId " +
                "WHERE  Achievements.Id = @Id";

            dbConnection.Open();

            var achievements = await dbConnection.QueryAsync<Achievement, Category, Achievement>(
                sql,
                (achievement, category) =>
                {
                    achievement.Category = category;

                    return achievement;
                },
                splitOn: "CategoryId",
                param: new { id }
            );

            return achievements.FirstOrDefault();
        }

        public async Task Update(Achievement item)
        {
            await using var dbConnection = new SqlConnection(_connectionString);

            const string query = "UPDATE Achievements " +
                                 "SET    CategoryId = @CategoryId, " +
                                 "       [Name] = @Name, " +
                                 "       Details = @Details, " +
                                 "       IsSecret = @IsSecret " +
                                 "WHERE  Id = @Id";

            dbConnection.Open();
            await dbConnection.ExecuteAsync(query, new { item.Id, CategoryId = item.Category.Id, item.Name, item.Details, item.IsSecret });
        }
    }
}
