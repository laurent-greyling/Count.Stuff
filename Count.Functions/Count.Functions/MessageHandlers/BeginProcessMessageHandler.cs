using System;
using System.Threading.Tasks;
using Count.Functions.Models;
using Count.Functions.Services;
using SimpleInjector;

namespace Count.Functions.MessageHandlers
{
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

        public Task HandleAsync(ManagementModel message)
        {
            throw new NotImplementedException();
        }
    }
}
