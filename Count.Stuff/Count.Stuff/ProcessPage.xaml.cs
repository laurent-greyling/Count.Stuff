using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Count.Stuff
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProcessPage : ContentPage
	{
		public ProcessPage (string processId)
		{
			InitializeComponent ();
		}
	}
}