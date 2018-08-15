﻿using Count.Stuff.Entities;
using Count.Stuff.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Count.Stuff
{
	public partial class MainPage : ContentPage
	{
        public GetProcessListViewModel ProcessIds { get; set; }

        public MainPage()
		{
			InitializeComponent();

            ProcessIds = new GetProcessListViewModel();

            BindingContext = ProcessIds;
        }

        /// <summary>
        /// Add process to the list and start counting function
        /// </summary>
        /// <returns></returns>
        public async Task Add_New_Process()
        {
            await new CreateProcessViewModel().Create();

            ProcessIds = new GetProcessListViewModel();

            BindingContext = ProcessIds;
        }

        public async Task Navigate_To_Process(object sender, ItemTappedEventArgs e)
        {
            var item = sender as ListView;
            var selectedItem = item.SelectedItem as ProcessEntity;
            var processId = selectedItem.ProcessId;

            await Navigation.PushAsync(new ProcessPage(processId));
        }
    }
}
