using Count.Functions.Models;
using Count.Functions.StartUpProcess;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleInjector;
using System.Threading.Tasks;

namespace Count.Functions.Functions
{
    /// <summary>
    /// Function to handle start process and counting process
    /// </summary>
    public static class ManagementFunction
    {
        [FunctionName("ManagementFunction")]
        public static async Task Run([QueueTrigger("management")]string queueMessage, ILogger log)
        {
            var container = new Container();
            var startUp = new StartUp(container);

            var message = JsonConvert.DeserializeObject<ManagementModel>(queueMessage);
            await startUp.RunAsync(message).ConfigureAwait(false);            
        }
    }
}
