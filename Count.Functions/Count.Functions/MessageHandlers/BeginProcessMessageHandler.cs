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
        /// This will then start queueing messages for the next handler to pick up and process the data
        /// </summary>
        public BeginProcessMessageHandler()
        {
            container = new Container();
            _azureService = container.GetInstance<AzureService>();
            _restService = container.GetInstance<RestService>();
        }

        public async Task HandleAsync(ManagementModel message)
        {
            //TODO: This could be a helper class as it is shared between BeginProcess and SaveDetails messagehandlers
            var apiUrl = Environment.GetEnvironmentVariable("ApiBaseUrl");
            var apiKey = Environment.GetEnvironmentVariable("ApiKey");
            var request = message.IsGardenSearch
                ? $"{apiUrl}{apiKey}{AppConst.SearchQuery}/tuin/&page={message.ObjectNumber}&pagesize=25"
                : $"{apiUrl}{apiKey}{AppConst.SearchQuery}&page={message.ObjectNumber}&pagesize=25";
            var workDetails = new WorkerModel();

            try
            {
                var result = await _restService.GetAsync(request);
                workDetails = JsonConvert.DeserializeObject<WorkerModel>(result);
                await LogStartProgressAsync(message.ProcessId, workDetails, message.IsGardenSearch, false);

                for (int countToEnd = 1; countToEnd <= workDetails.Paging.AantalPaginas; countToEnd++)
                {
                    await SendBasicSearchMessageAsync(message.ProcessId, countToEnd, workDetails, message.IsGardenSearch);
                }

                //If at end restart process with search criteria for garden.
                if (!message.IsGardenSearch)
                {
                    await SendSearchCriteriaMessage(message.ProcessId);
                }
            }
            catch (Exception ex)
            {
                await LogStartProgressAsync(message.ProcessId, workDetails, message.IsGardenSearch, true);
                //TODO: Need to log something here, or send message back to client to inform of failure
                //Would preferably log exceptions into Application Insights. This will allow us to query AI and 
                //see what went wrong on any given day. As AI cost a bit of money, I did not create this resource in Azure

                //Example Query for AI for exceptions
                //exceptions | where timestamp < ago(7d) 


                //If this is seen as warning or fatal level (at minimum I see this as a warning that process didn't start),
                //one can also create an Alert in Azure that when AI has a severity level of 2 or 3 someone will get an email or sms

                //Code recently done for logging these types of warnings that could be implemented for exceptions
                //AI Query = traces | where (severityLevel == 3 or severityLevel == 2 ) and (message startswith 'FATAL' or message startswith 'WARNING')
                //private bool IsValidSasToken(OperationalSettings connectionSettings)
                //{
                //    var productionConnection = connectionSettings.ProductionConnection;

                //    var indexOfExpirationDate = productionConnection.IndexOf("se=");

                //    if (indexOfExpirationDate < 0)
                //    {
                //        //severitylevel 3 in AI
                //        _logger.Fatal("FATAL: no valid signed expiry date found");
                //        return false;
                //    }
                //    var expiryDate = productionConnection.Substring((indexOfExpirationDate + 3), 10);

                //    var expirationDate = DateTime.ParseExact(
                //        expiryDate,
                //        "yyyy-MM-dd",
                //        CultureInfo.InvariantCulture);

                //    var today = DateTime.UtcNow.Date;

                //    var expirationWarning = DateTime.Compare(expirationDate.AddDays(-10), today);
                //    var expirationError = DateTime.Compare(expirationDate, today);

                //    if (expirationError == 0)
                //    {
                //        _logger.Fatal("FATAL: sas connection is expiring today {Today}", today);
                //        return false;
                //    }

                //    if (expirationError < 0)
                //    {
                //        _logger.Fatal("FATAL: sas token has expired on {ExpirationDate}", expirationDate);
                //        return false;
                //    }

                //    if (expirationWarning <= 0)
                //    {
                //        //severitylevel 2 in AI
                //        _logger.Warning("WARNING: sas token will expire in a few days");
                //    }

                //    return true;
                //}
                throw;
            }
        }

        /// <summary>
        /// This will send message to be picked up by savedetails handler
        /// Save details will use this message to get the correct page and navigate through the objects
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="countToEnd"></param>
        /// <param name="workDetails"></param>
        /// <param name="isGardenSearch"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will queue a new message for Begin process where the process will restart with Garden added as search criteria
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
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
        /// Client can check this table to know the status of their process
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="workerModel"></param>
        /// <param name="isGardenSearch"></param>
        /// <returns></returns>
        private async Task LogStartProgressAsync(string processId,
            WorkerModel workerModel,
            bool isGardenSearch,
            bool isError)
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
                NumberOfGardenObjects = isGardenSearch ? workerModel.TotaalAantalObjecten : 0,
                InErrorState = isError
            };

            await _azureService.InsertOrMergeAsync(AppConst.ProgressTable, entity);
        }
    }
}
