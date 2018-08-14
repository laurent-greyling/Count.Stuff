using Count.Stuff.Entities;
using Count.Stuff.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Count.Stuff.ViewModels
{
    public class GetProcessListViewModel : INotifyPropertyChanged
    {
        readonly ISqliteService<ProcessEntity> _sqlite;

        public IEnumerable<ProcessEntity> _processIds { get; set; }

        public IEnumerable<ProcessEntity> ProcessIds
        {
            get
            {
                return _processIds;
            }
            set
            {
                if (_processIds != value)
                {
                    _processIds = value;
                    OnPropertyChanged("ProcessIds");
                }
            }
        }

        /// <summary>
        /// Get list of processes created and order them by putting the latest process at the top of the list
        /// </summary>
        public GetProcessListViewModel()
        {
            _sqlite = DependencyService.Get<ISqliteService<ProcessEntity>>();            
            _processIds = Get();
        }

        private IEnumerable<ProcessEntity> Get()
        {
            var entities = _sqlite.Get();
            return entities.OrderByDescending(x => x.Id).ToList() ?? new List<ProcessEntity>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
