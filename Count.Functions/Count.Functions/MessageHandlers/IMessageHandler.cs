using Count.Functions.Models;
using System.Threading.Tasks;

namespace Count.Functions.MessageHandlers
{
    interface IMessageHandler
    {
        /// <summary>
        /// Handle message to direct queue info to the correct operations
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task HandleAsync(ManagementModel message);
    }
}
