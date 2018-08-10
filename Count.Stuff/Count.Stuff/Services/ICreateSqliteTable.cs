using SQLite;

namespace Count.Stuff.Services
{
    public interface ICreateSqliteTable
    {
        /// <summary>
        /// Creates the Sqlite table and return the connection to the table for use
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        SQLiteConnection Create<T>();
    }
}
