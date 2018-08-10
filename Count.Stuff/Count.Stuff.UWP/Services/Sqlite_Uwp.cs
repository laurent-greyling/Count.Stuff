using Count.Stuff.Services;
using Count.Stuff.UWP.Services;
using SQLite;
using System.IO;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sqlite_Uwp))]
namespace Count.Stuff.UWP.Services
{
    public class Sqlite_Uwp : ISqlite
    {
        //Creates the Sqlite DB and return the connection string for Windows platform
        public SQLiteConnection GetConnection()
        {
            //Ofcourse this should be a much better name, but for the sake of this example HoldStuff will do
            var fileName = "HoldStuff.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            return new SQLiteConnection(path);
        }
    }
}
