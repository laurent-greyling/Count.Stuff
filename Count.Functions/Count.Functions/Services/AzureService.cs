
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    public class AzureService<T> : IAzureService<T>
    {
        CloudTable _table;
        CloudQueue _queue;
        CloudStorageAccount _account;
        CloudTableClient _tableClient;
        CloudQueueClient _queueClient;

        public AzureService(string tableName, string queueName)
        {
            _account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            _tableClient = _account.CreateCloudTableClient();
            _queueClient = _account.CreateCloudQueueClient();

            _table = _tableClient.GetTableReference(tableName);
            _queue = _queueClient.GetQueueReference(queueName);

        }

        public Task InsertOrMerge(T entity)
        {
            throw new System.NotImplementedException();
        }

        public Task SendMessage(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
