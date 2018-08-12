﻿
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    public class AzureService : IAzureService
    {
        CloudStorageAccount _account;

        public AzureService()
        {
            _account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        }

        public async Task InsertOrMergeAsync(string tableName, List<ITableEntity> entities)
        {
            //You will notice there is no CreateIfNotExist, this is because I assume that the table already exist
            //Usually when you deploy an Azure service you will use the ARM template to create all the resources you need.
            //If this is not the case, feel free to add await _table.CreateIfNotExistAsync();
            //Same goes for the queue messsages
            var tableClient = _account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            
            var batchOperation = new TableBatchOperation();

            foreach (var entity in entities)
            {
                batchOperation.InsertOrMerge(entity);
            }

            await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            var cloudMessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(cloudMessage).ConfigureAwait(false);
        }
    }
}
