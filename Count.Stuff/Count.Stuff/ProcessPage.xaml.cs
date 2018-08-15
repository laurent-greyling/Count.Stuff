using Count.Stuff.Helpers;
using Count.Stuff.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Count.Stuff
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProcessPage : ContentPage
	{
        public  GetProgressViewModel Progress { get; set; }
        public string _processId;

        /// <summary>
        /// This page allows you to navigate to the top ten agents with and without garden
        /// The buttons will only be available if the process has completed. 
        /// </summary>
        /// <param name="processId"></param>
		public ProcessPage (string processId)
		{
            _processId = processId;
            Progress = new GetProgressViewModel(processId);

            InitializeComponent ();

            BindingContext = Progress;
        }

        /// <summary>
        /// Refresh the page to check if process completed
        /// </summary>
        private void Refresh_Status()
        {
            Progress = new GetProgressViewModel(_processId);
            BindingContext = Progress;
        }

        /// <summary>
        /// Will navigate to top results page and show the overall top results
        /// </summary>
        /// <returns></returns>
        private async Task Overall_Results()
        {
            await Navigation.PushAsync(new TopResultsPage(_processId, false));
        }

        /// <summary>
        /// Will navigate to top results page and show the garden top results
        /// </summary>
        /// <returns></returns>
        private async Task Garden_Results()
        {
            await Navigation.PushAsync(new TopResultsPage(_processId, true));
        }
    }
}