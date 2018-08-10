using System.IO;
using Count.Stuff.Droid.Services;
using Count.Stuff.Services;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sqlite_Droid))]
namespace Count.Stuff.Droid.Services
{
    public class Sqlite_Droid : ISqlite
    {
        //Creates the Sqlite DB and return the connection string for Android Platform
        public SQLiteConnection GetConnection()
        {
            //Ofcourse this should be a much better name, but for the sake of this example HoldStuff will do
            var fileName = "HoldStuff.db3";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, fileName);

            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}