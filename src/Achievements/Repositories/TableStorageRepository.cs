using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Achievements.Repositories
{
    public class TableStorageRepository<T> where T : TableEntity, new()
    {
        private readonly string _connectionString;
        private readonly CloudTable _table;

        public TableStorageRepository(IConfiguration config, string tableName)
        {
            _connectionString = config.GetConnectionString("TableStorage");
            _table = GetTable(tableName);
        }

        public TableStorageRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _table = GetTable(tableName);
        }

        protected CloudTable GetTable(string tableName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);
                var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
                return tableClient.GetTableReference(tableName);
            }
            catch (FormatException)
            {
                Log.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Log.Error("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetEntities(string partitionKey = null)
        {
            try
            {
                var table = GetTable(_table.Name);

                TableContinuationToken token = null;
                var entities = new List<T>();

                var query =
                    partitionKey != null
                        ? new TableQuery<T>().Where(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                partitionKey))
                        : new TableQuery<T>();

                do
                {
                    var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                    entities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);

                return entities;
            }
            catch (StorageException ex)
            {
                Log.Error("Error retrieving players from table storage: {@ex}", ex);
                throw;
            }
        }

        public async Task<T> GetEntity(string partitionKey, string rowKey)
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                var result = await _table.ExecuteAsync(retrieveOperation);
                var entity = result.Result as T;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return entity;
            }
            catch (StorageException ex)
            {
                Log.Error("Error retrieving data from table storage: {@ex}", ex);
                throw;
            }
        }

        public async Task<T> InsertOrMergeEntity(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                var result = await _table.ExecuteAsync(insertOrMergeOperation);
                var insertedEntity = result.Result as T;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedEntity;
            }
            catch (StorageException ex)
            {
                Log.Error("Error updating data in table storage: {@ex}", ex);
                throw;
            }
        }

        public async Task InsertOrMergeEntities(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                var tableBatchOperation = new TableBatchOperation();
                entities.ToList().ForEach(x => tableBatchOperation.Add(TableOperation.InsertOrMerge(x)));

                await _table.ExecuteBatchAsync(tableBatchOperation);
            }
            catch (StorageException ex)
            {
                Log.Error("Error updating batch of data in table storage: {@ex}", ex);
                throw;
            }
        }

        public async Task DeleteEntity(T deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException(nameof(deleteEntity));
                }

                var deleteOperation = TableOperation.Delete(deleteEntity);
                var result = await _table.ExecuteAsync(deleteOperation);

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (StorageException ex)
            {
                Log.Error("Error removing data from table storage: {@ex}", ex);
                throw;
            }
        }

        public async Task DeleteEntities(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null || !entities.Any())
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                // Batches of 100 as that is limit
                for (var i = 0; i < entities.Count(); i += 100)
                {
                    var tableBatchOperation = new TableBatchOperation();

                    entities.Skip(i).Take(100).ToList().ForEach(x => tableBatchOperation.Add(TableOperation.Delete(x)));

                    var result = await _table.ExecuteBatchAsync(tableBatchOperation);

                    Log.Debug($"Deleted Records @ {result.RequestCharge} ");
                }
            }
            catch (StorageException ex)
            {
                Log.Error("Error removing batch from table storage: {@ex}", ex);
                throw;
            }
        }
    }
}
