using System;
using System.Threading.Tasks;
using Count.Functions.Entities;
using Count.Functions.Models;
using Count.Functions.Services;
using Newtonsoft.Json;
using SimpleInjector;

namespace Count.Functions.MessageHandlers
{
    public class SaveDetailsMessageHandler : IMessageHandler
    {
        readonly Container container;
        readonly IAzureService _azureService;
        readonly IRestService _restService;

        /// <summary>
        /// Will get the page number from message and send request to api for the page needed
        /// Retrieve objects from this page and save needed info into azure table for client to display to user
        /// Count the occurence of agents (Basically)
        /// </summary>
        public SaveDetailsMessageHandler()
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

            var progressCounter = 0;
            var entities = new ObjectsModel();

            try
            {
                //Pause between requests to try and mitigate the request per minute
                await Task.Delay(600);
                var result = await _restService.GetAsync(request);
                entities = JsonConvert.DeserializeObject<ObjectsModel>(result);

                foreach (var item in entities.Objects)
                {
                    progressCounter++;
                    //Check if Item is available for sale - Not sure if verhuurd will count as not for sale. If it does, can add item.IsVerhuurd
                    if (item.IsVerkocht)
                    {
                        continue;
                    }

                    await ProcessAgentAsync(item, message);                   
                }
            }
            catch (Exception ex)
            {
                progressCounter--;
                //Log this in Application Insights or some logger - see comment in BeginProcessMessageHandler
                //What also need to still be handled extra in this exception is that when failed, a method similar to ProcessAgent need to remove the counts 
                //of already added.
                //Or we can keep a temp (cache) record of agents that were succesfully added and make sure on retry they do not get added again.
                //This is the main reason for not having the functions app retry (see host.json) at this point as there is no current logic to not add duplicates. For now 
                //we just continue and counts will not be updated, so process will not be seen as finished, but we can handle this on the client side for now.
                throw;
            }

            //Update Progress table
            var currentProgress = await _azureService.RetrieveEntityAsync<ProgressEntity>(AppConst.ProgressTable, AppConst.CountProgressPartitionKey, message.ProcessId);
            var progress = (ProgressEntity)currentProgress.Result;
            progress.NormalProgress = message.IsGardenSearch ? progress.NormalProgress : progress.NormalProgress + progressCounter;
            progress.GardenProgress = message.IsGardenSearch ? progress.GardenProgress + progressCounter : progress.GardenProgress;

            //Update this so if someone added or removed and object the count will still match up at end to indigate process have completed
            //this does not safegaurd against something removed once already iterated over.
            progress.NumberOfNormalObjects = message.IsGardenSearch ? progress.NumberOfNormalObjects : entities.TotaalAantalObjecten;
            progress.NumberOfGardenObjects = message.IsGardenSearch ? entities.TotaalAantalObjecten : progress.NumberOfGardenObjects;

            progress.IsNormalSearchDone = progress.NumberOfNormalObjects == progress.NormalProgress && progress.NormalProgress > 0;
            progress.IsGardenSearchDone = progress.NumberOfGardenObjects == progress.GardenProgress && progress.GardenProgress > 0;

            await _azureService.InsertOrMergeAsync(AppConst.ProgressTable, progress);
        }

        /// <summary>
        /// Process agent info and add to count
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ProcessAgentAsync(ObjectModel item, ManagementModel message)
        {
            //Initially removed these characters to keep string safe for table storage rowkey. Not really needed anymore as this is not rowkey anymore
            var makelaarNaam = item.MakelaarNaam.Replace("/", "").Replace(@"\", "");
            var agentResult = await _azureService.RetrieveEntityAsync<AgentsEntity>(AppConst.AgentsTable, message.ProcessId, item.MakelaarId.ToString());

            var agentEntity = (AgentsEntity)agentResult.Result;

            if (agentEntity == null)
            {
                var agent = new AgentsEntity
                {
                    PartitionKey = message.ProcessId,
                    RowKey = item.MakelaarId.ToString(),
                    GardenCount = message.IsGardenSearch ? 1 : 0,
                    NormalCount = !message.IsGardenSearch ? 1 : 0,
                    AgentName = makelaarNaam
                };

                await _azureService.InsertOrMergeAsync(AppConst.AgentsTable, agent);

                return;
            }

            if (message.IsGardenSearch)
            {
                agentEntity.GardenCount += 1;
            }
            else
            {
                agentEntity.NormalCount += 1;
            }

            await _azureService.InsertOrMergeAsync(AppConst.AgentsTable, agentEntity);
        }
    }
}
