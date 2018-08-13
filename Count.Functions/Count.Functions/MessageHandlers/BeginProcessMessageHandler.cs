using System;
using System.Threading;
using System.Threading.Tasks;
using Count.Functions.Entities;
using Count.Functions.Models;
using Count.Functions.Services;
using Newtonsoft.Json;
using SimpleInjector;

namespace Count.Functions.MessageHandlers
{
    /// <summary>
    /// The Begin Process is responsible to see how many pages and objects there are to work through and then send a message 
    /// to the worker process to start getting the needed info for counting to happen.
    /// </summary>
    public class BeginProcessMessageHandler : IMessageHandler
    {        
        readonly Container container;
        readonly IAzureService _azureService;
        readonly IRestService _restService;

        /// <summary>
        /// Message Handler to start the process of finding the objects to count
        /// This process will get the first object, see how many objects exist and need to be navigated
        /// This will then start queueing message for the next handler to pick up and process the data
        /// </summary>
        public BeginProcessMessageHandler()
        {
            container = new Container();
            _azureService = container.GetInstance<AzureService>();
            _restService = container.GetInstance<RestService>();
        }

        public async Task HandleAsync(ManagementModel message)
        {
            var apiUrl = Environment.GetEnvironmentVariable("ApiBaseUrl");
            var apiKey = Environment.GetEnvironmentVariable("ApiKey");
            var request = message.IsGardenSearch
                ? $"{apiUrl}{apiKey}{AppConst.SearchQuery}/tuin/&page={message.ObjectNumber}&pagesize=25"
                : $"{apiUrl}{apiKey}{AppConst.SearchQuery}&page={message.ObjectNumber}&pagesize=25";

            try
            {
                var result = await _restService.GetAsync(request);
                var workDetails = JsonConvert.DeserializeObject<WorkerModel>(result);
                await LogStartProgressAsync(message.ProcessId, workDetails, message.IsGardenSearch);

                for (int countToEnd = 0; countToEnd <= workDetails.Paging.AantalPaginas; countToEnd++)
                {
                    await SendBasicSearchMessageAsync(message.ProcessId, countToEnd, workDetails, message.IsGardenSearch);

                    //If at end restart process with search criteria for garden. 
                    //Would like not do to this, but cannot find element in object for hasgarden.
                    if (countToEnd == workDetails.Paging.AantalPaginas && !message.IsGardenSearch)
                    {
                        await SendSearchCriteriaMessage(message.ProcessId);
                    }
                }
            }
            catch (Exception ex)
            {
                var t = ex;
                //TODO: Need to log something here, or send message back to client to inform of failure
                throw;
            }
        }

        private async Task SendBasicSearchMessageAsync(
            string processId,
            int countToEnd,
            WorkerModel workDetails,
            bool isGardenSearch)
        {
            var messageDetails = new ManagementModel
            {
                ProcessId = processId,
                MessageType = MessageType.SaveDetails.ToString(),
                ObjectNumber = countToEnd,
                NumberOfObjects = workDetails.TotaalAantalObjecten,
                NumberOfPages = workDetails.Paging.AantalPaginas,
                IsGardenSearch = isGardenSearch
            };

            var messageContent = JsonConvert.SerializeObject(messageDetails);

            await _azureService.SendMessageAsync(AppConst.ManagementQueueName, messageContent);
        }

        private async Task SendSearchCriteriaMessage(string processId)
        {
            var messageDetails = new ManagementModel
            {
                ProcessId = processId,
                MessageType = MessageType.BeginProcess.ToString(),
                ObjectNumber = 1,
                IsGardenSearch = true
            };

            var messageContent = JsonConvert.SerializeObject(messageDetails);
            await _azureService.SendMessageAsync(AppConst.ManagementQueueName, messageContent);
        }

        /// <summary>
        /// Log when process starts, this will be updated by worker.
        /// Client can check this table to know the status of thr process
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="workerModel"></param>
        /// <param name="isGardenSearch"></param>
        /// <returns></returns>
        private async Task LogStartProgressAsync(string processId, WorkerModel workerModel, bool isGardenSearch)
        {
            try
            {
                //This is to check if an already existing process has started/completed for specific process.
                //This is also to gaurd against resetting the normal process once garden process starts
                var currentProgressEntity = await _azureService.RetrieveEntityAsync<ProgressEntity>(
                    AppConst.ProgressTable,
                    AppConst.CountProgressPartitionKey,
                    processId);

                var progress = (ProgressEntity)currentProgressEntity.Result;

                var entity = new ProgressEntity
                {
                    PartitionKey = AppConst.CountProgressPartitionKey,
                    RowKey = processId,
                    NormalProgress = progress == null ? 0 : progress.NormalProgress,
                    GardenProgress = progress == null ? 0 : progress.GardenProgress,
                    NumberOfNormalObjects = progress == null ? workerModel.TotaalAantalObjecten : progress.NumberOfNormalObjects,
                    NumberOfGardenObjects = isGardenSearch ? workerModel.TotaalAantalObjecten : 0
                };

                await _azureService.InsertOrMergeAsync(AppConst.ProgressTable, entity);
            }
            catch (Exception e)
            {
                //TODO: log error, send message back
                throw;
            }
        }
    }
}
