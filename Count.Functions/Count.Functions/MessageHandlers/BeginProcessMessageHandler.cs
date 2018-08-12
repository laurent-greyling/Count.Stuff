using System;
using System.Threading;
using System.Threading.Tasks;
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
                var countToEnd = 0;
                var result = await _restService.GetAsync(request);
                var workDetails = JsonConvert.DeserializeObject<WorkerModel>(result);

                do
                {
                    countToEnd++;

                    await SendBasicSearchMessageAsync(countToEnd, workDetails, message.IsGardenSearch);

                    //If at end restart process with search criteria for garden. 
                    //Would like not do to this, but cannot find element in object for hasgarden.
                    
                    if (countToEnd == workDetails.Paging.AantalPaginas)
                    {
                        //Do not do this again, need another criteria in if else we will end up in infinate loop
                        await SendSearchCriteriaMessage();
                    }

                } while (countToEnd < workDetails.Paging.AantalPaginas);
            }
            catch (Exception)
            {
                //TODO: Need to log something here, or send message back to client to inform of failure
                throw;
            }
        }

        private async Task SendBasicSearchMessageAsync(int countToEnd, WorkerModel workDetails, bool isGardenSearch)
        {
            var messageDetails = new ManagementModel
            {
                MessageType = MessageType.SaveDetails.ToString(),
                ObjectNumber = countToEnd,
                NumberOfObjects = workDetails.TotaalAantalObjecten,
                NumberOfPages = workDetails.Paging.AantalPaginas,
                IsGardenSearch = isGardenSearch
            };

            var messageContent = JsonConvert.SerializeObject(messageDetails);

            //Want to block before sending the message. This is in an effort (1st level) to limit the request per minute for API calls
            //Use thread sleep to block thread. Pausing with timer elapsed will pause and not block so paralallism of function will send messsage multiple times.
            Thread.Sleep(500);
            await _azureService.SendMessageAsync(AppConst.ManagementQueueName, messageContent);
        }

        private async Task SendSearchCriteriaMessage()
        {
            var messageDetails = new ManagementModel
            {
                MessageType = MessageType.BeginProcess.ToString(),
                ObjectNumber = 1,
                IsGardenSearch = true
            };

            var messageContent = JsonConvert.SerializeObject(messageDetails);
            await _azureService.SendMessageAsync(AppConst.ManagementQueueName, messageContent);
        }
    }
}
