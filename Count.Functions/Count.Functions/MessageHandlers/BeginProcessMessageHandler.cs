using System;
using System.Threading.Tasks;
using Count.Functions.Models;

namespace Count.Functions.MessageHandlers
{
    public class BeginProcessMessageHandler : IMessageHandler
    {
        public BeginProcessMessageHandler()
        {
        }

        public Task HandleAsync(ManagementModel message)
        {
            throw new NotImplementedException();
        }
    }
}
