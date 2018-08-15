
using Count.Stuff.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Count.Stuff
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TopResultsPage : ContentPage
	{
        public GetTopResultsViewModel Agents { get; set; }
        private bool _isGardenResults;
		public TopResultsPage (string processId, bool isGardenResults)
		{
            _isGardenResults = isGardenResults;
            Agents = new GetTopResultsViewModel(processId, isGardenResults);
			InitializeComponent ();

            BindingContext = Agents;
		}

        private async Task Order_Results()
        {
            try
            {
                if (Agents.Agents.IsSuccessfullyCompleted)
                {
                    TopResults.ItemsSource = _isGardenResults 
                        ? Agents.Agents.Result.OrderByDescending(o => o.GardenCount).Take(10)
                        : Agents.Agents.Result.OrderByDescending(o => o.NormalCount).Take(10);
                }
            }
            catch (System.Exception)
            {
                await DisplayAlert("Oeps", "Could not retrieve results, please try again later", "Ok");
            }
        }

    }
}