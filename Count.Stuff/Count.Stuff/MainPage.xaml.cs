using Count.Stuff.Entities;
using Count.Stuff.Models;
using Count.Stuff.Services;
using Count.Stuff.ViewModels;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Count.Stuff
{
	public partial class MainPage : ContentPage
	{
        readonly ISqliteService<ProcessEntity> _sqlite;
        readonly IAzureService _azure;

        public MainPage()
		{
			InitializeComponent();

            _sqlite = DependencyService.Get<ISqliteService<ProcessEntity>>();
            _azure = DependencyService.Get<IAzureService>();
        }

        public async Task Add_New_Process()
        {
            await new CreateProcessViewModel().Create();
        }
    }
}
