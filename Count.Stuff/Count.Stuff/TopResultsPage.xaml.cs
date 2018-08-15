
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

        /// <summary>
        /// Page that will hold the top results depending on where navigation comes from
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="isGardenResults"></param>
		public TopResultsPage (string processId, bool isGardenResults)
		{
            _isGardenResults = isGardenResults;
            Agents = new GetTopResultsViewModel(processId, isGardenResults);
			InitializeComponent ();

            BindingContext = Agents;
		}

        /// <summary>
        /// Depending if garden results or overal normal results is requested, this will order list and take top 10 of ordered list
        /// </summary>
        /// <returns></returns>
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