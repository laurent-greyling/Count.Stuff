using Count.Stuff.Helpers;
using Count.Stuff.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Count.Stuff
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProcessPage : ContentPage
	{
        public  GetProgressViewModel Progress { get; set; }
        public string _processId;
		public ProcessPage (string processId)
		{
            _processId = processId;
            Progress = new GetProgressViewModel(processId);

            InitializeComponent ();

            BindingContext = Progress;
        }

        private void Refresh_Status()
        {
            Progress = new GetProgressViewModel(_processId);
            BindingContext = Progress;
        }
    }
}