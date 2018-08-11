using System;
using System.Threading.Tasks;
using Count.Functions.MessageHandlers;
using Count.Functions.Models;
using SimpleInjector;

namespace Count.Functions.StartUpProcess
{
    public class StartUp : IStartUp
    {
        readonly Container _container;
        public StartUp(Container container)
        {
            _container = container;
        }

        public async Task RunAsync(ManagementModel message)
        {
            var messageType = $"Count.Functions.MessageHandlers.{message.MessageType}MessageHandler";
            Type type = Type.GetType(messageType);

            _container.Register(type);
            var instance = (IMessageHandler)_container.GetInstance(type);
            await instance.HandleAsync(message).ConfigureAwait(false);
        }
    }
}
