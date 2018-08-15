using Count.Stuff.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Count.Stuff.Services
{
    public class AzureService : IAzureService
    {
        CloudStorageAccount _account;

        public AzureService()
        {
            _account = CloudStorageAccount.Parse(AppConst.ConnectionString);
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            var cloudMessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(cloudMessage).ConfigureAwait(false);
        }

        public async Task<T> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);
                TableQuery<T> query = new TableQuery<T>()
                    .Where(TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                        ));

                var entity = await table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);
                return entity.Results.FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                var t = ex;
                throw;
            }
            
        }

        public async Task<List<T>> RetrieveEntitiesAsync<T>(string tableName, string partitionKey) where T : ITableEntity, new()
        {
            var tableClient = _account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            TableQuery<T> query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var entities = new List<T>();
            TableContinuationToken token = null;

            do
            {
                var segmented = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                token = segmented.ContinuationToken;
                entities.AddRange(segmented);

            } while (token != null);

            return entities;
        }
    }
}
