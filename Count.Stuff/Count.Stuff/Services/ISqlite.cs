using SQLite;

namespace Count.Stuff.Services
{
    /// <summary>
    /// For Android <see cref="Droid.Services"/>
    /// </summary>
    interface ISqlite
    {
        SQLiteConnection GetConnection();
    }
}
