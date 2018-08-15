
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

        public NotifyTaskCompletion<ProgressEntity> _progress { get; set; }

        public NotifyTaskCompletion<ProgressEntity> Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged("Progress");
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

            Progress = new NotifyTaskCompletion<ProgressEntity>(_azure.RetrieveEntityAsync<ProgressEntity>(AppConst.ProgressTable, AppConst.CountProgressPartitionKey, processId));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
