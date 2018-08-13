using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            try
            {
                //Pause between requests to try and mitigate the request per minute
                await Task.Delay(1600);
                var result = await _restService.GetAsync(request);
                var entities = JsonConvert.DeserializeObject<ObjectsModel>(result);

                foreach (var item in entities.Objects)
                {
                    //Check if Item is available for sale - Not sure if verhuurd will count as not for sale. If it does, can add item.IsVerhuurd
                    if (item.IsVerkocht)
                    {
                        continue;
                    }

                    var makelaars = entities.Objects.Where(a => a.MakelaarId == item.MakelaarId);

                    var makelaarNaam = item.MakelaarNaam.Replace("/", "").Replace(@"\", "");
                    var agentResult = await _azureService.RetrieveEntityAsync<AgentsEntity>(AppConst.AgentsTable, item.MakelaarId.ToString(), makelaarNaam);

                    var agentEntity = (AgentsEntity)agentResult.Result;

                    var agent = new AgentsEntity
                    {
                        PartitionKey = item.MakelaarId.ToString(),
                        RowKey = makelaarNaam,
                    };

                    if (message.IsGardenSearch)
                    {
                        agent.GardenCount = agentEntity == null ? 1 : agentEntity.GardenCount + 1;
                        if (agentEntity!=null)
                        {
                            agent.NormalCount = agentEntity.NormalCount;
                        }
                    }
                    else
                    {
                        agent.NormalCount = agentEntity == null ? 1 : agentEntity.NormalCount + 1;
                        if (agentEntity != null)
                        {
                            agent.GardenCount = agentEntity.GardenCount;
                        }
                    }

                    await _azureService.InsertOrMergeAsync(AppConst.AgentsTable, agent);
                    
                }
                
            }
            catch (Exception ex)
            {
                var t = ex;
                //Log this if something went wrong and inform counts can be off as result of this/ or return nothing and have process restarted
                throw;
            }
        }
    }
}
