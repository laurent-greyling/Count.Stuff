using System;
using System.Threading.Tasks;
using Count.Functions.Models;
using Count.Functions.Services;
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

        public Task HandleAsync(ManagementModel message)
        {
            var apiUrl = Environment.GetEnvironmentVariable("ApiBaseUrl");
            var apiKey = Environment.GetEnvironmentVariable("ApiKey");
            var request = $"{apiUrl}{apiKey}{AppConst.SearchQuery}&page={message.ObjectNumber}&pagesize=25";



            throw new NotImplementedException();
        }
    }
}
