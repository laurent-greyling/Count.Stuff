using SQLite;

namespace Count.Stuff.Services
{
    /// <summary>
    /// For Android <see cref="Droid.Services"/>
    /// For Uwp <see cref="UWP.Services"/>
    /// </summary>
    interface ISqlite
    {
        SQLiteConnection GetConnection();
    }
}
