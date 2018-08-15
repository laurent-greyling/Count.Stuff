
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Count.Stuff.Services
{
    /// <summary>
    /// Service to access azure
    /// </summary>
    public interface IAzureService
    {
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
        Task<T> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new();

        /// <summary>
        /// Get all entities in a partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<List<T>> RetrieveEntitiesAsync<T>(string tableName, string partitionKey) where T : ITableEntity, new();
    }
}
