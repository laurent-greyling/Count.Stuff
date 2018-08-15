using Count.Stuff.Entities;
using Count.Stuff.Models;
using Count.Stuff.Services;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Count.Stuff.ViewModels
{
    public class CreateProcessViewModel : INotifyPropertyChanged
    {
        readonly ISqliteService<ProcessEntity> _sqlite;
        readonly IAzureService _azure;

        /// <summary>
        /// Create the process and send the message to the queue for the function to pick up and start processing
        /// </summary>
        public CreateProcessViewModel()
        {
            _sqlite = DependencyService.Get<ISqliteService<ProcessEntity>>();
            _azure = DependencyService.Get<IAzureService>();            
        }

        public async Task Create()
        {
            var processId = Guid.NewGuid().ToString();
            var messageBody = new ManagementModel
            {
                ProcessId = processId,
                MessageType = MessageType.BeginProcess.ToString(),
                ObjectNumber = 1
            };

            var message = JsonConvert.SerializeObject(messageBody);

            _sqlite.Add(new ProcessEntity { ProcessId = processId });
            await _azure.SendMessageAsync(AppConst.ManagementQueueName, message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
