using SQLite;
using Xamarin.Forms;

namespace Count.Stuff.Services
{
    public class CreateSqliteTable : ICreateSqliteTable
    {
        private SQLiteConnection _connection;

        public CreateSqliteTable()
        {
            _connection = DependencyService.Get<ISqlite>().GetConnection();
        }

        public SQLiteConnection Create<T>()
        {
            _connection.CreateTable<T>();
            return _connection;
        }
    }
}
