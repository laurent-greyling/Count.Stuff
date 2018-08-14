
using Count.Stuff.Entities;
using Count.Stuff.Helpers;
using Count.Stuff.Models;
using Count.Stuff.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Count.Stuff.ViewModels
{
    public class GetProgressViewModel : INotifyPropertyChanged
    {
        readonly IAzureService _azure;

        public bool _isNormalDone { get; set; }

        public bool IsNormalDone
        {
            get
            {
                return _isNormalDone;
            }
            set
            {
                if (_isNormalDone != value)
                {
                    _isNormalDone = value;
                    OnPropertyChanged("IsNormalDone");
                }
            }
        }

        public bool _isGardenDone { get; set; }

        public bool IsGardenDone
        {
            get
            {
                return _isGardenDone;
            }
            set
            {
                if (_isGardenDone != value)
                {
                    _isGardenDone = value;
                    OnPropertyChanged("IsGardenDone");
                }
            }
        }

        /// <summary>
        /// Will check the progress table to see if task has completed and then allow you to see top counts
        /// </summary>
        /// <param name="processId"></param>
        public GetProgressViewModel(string processId)
        {
            _azure = DependencyService.Get<IAzureService>();

            Task.Run(async () => await Get(processId));
        }

        public async Task Get(string processId)
        {
            var progress = await _azure.RetrieveEntityAsync<ProgressEntity>(AppConst.ProgressTable, AppConst.CountProgressPartitionKey, processId);

            _isNormalDone = progress.NormalProgress == progress.NumberOfNormalObjects;
            _isGardenDone = progress.GardenProgress == progress.NumberOfGardenObjects;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
