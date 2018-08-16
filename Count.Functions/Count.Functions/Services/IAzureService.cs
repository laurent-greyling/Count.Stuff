
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    /// <summary>
    /// Service to Access azure services
    /// </summary>
    interface IAzureService
    {
        /// <summary>
        /// Azure Table Insert or merge batch
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertOrMergeAsync(string tableName, List<ITableEntity> entities);

        /// <summary>
        /// Azure Table Insert or merge
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertOrMergeAsync(string tableName, ITableEntity entity);

        /// <summary>
        /// Send a message to storage queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageAsync(string queueName, string message);

        /// <summary>
        /// Get table Entity
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<TableResult> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity;

        /// <summary>
        /// Get a blob lease in order to manage concurrency in table storage for paralell operations
        /// </summary>
        /// <returns></returns>
        Task<string> AcquireLeaseIdAsync();

        /// <summary>
        /// release the blob lease
        /// </summary>
        Task ReleaseLeaseAsync(string leaseId);
    }
}
