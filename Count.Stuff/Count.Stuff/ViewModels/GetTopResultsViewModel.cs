using Count.Stuff.Entities;
using Count.Stuff.Helpers;
using Count.Stuff.Models;
using Count.Stuff.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Count.Stuff.ViewModels
{
    public class GetTopResultsViewModel : INotifyPropertyChanged
    {
        readonly IAzureService _azure;

        public NotifyTaskCompletion<List<AgentsEntity>> _agents { get; set; }

        public NotifyTaskCompletion<List<AgentsEntity>> Agents
        {
            get
            {
                return _agents;
            }
            set
            {
                if (_agents != value)
                {
                    _agents = value;
                    OnPropertyChanged("Agents");
                }
            }
        }

        /// <summary>
        /// Will get the list of agents from the agents table
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="isGardenResults"></param>
        public GetTopResultsViewModel(string processId, bool isGardenResults)
        {
            _azure = DependencyService.Get<IAzureService>();

            _agents = new NotifyTaskCompletion<List<AgentsEntity>>(_azure.RetrieveEntitiesAsync<AgentsEntity>(AppConst.AgentsTable, processId));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
