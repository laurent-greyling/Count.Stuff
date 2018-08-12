
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    /// <summary>
    /// Service to Access azure services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IAzureService
    {
        /// <summary>
        /// Azure Table Insert or merge
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertOrMergeAsync(string tableName, List<ITableEntity> entities);

        /// <summary>
        /// Send a message to storage queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageAsync(string queueName, string message);
    }
}
