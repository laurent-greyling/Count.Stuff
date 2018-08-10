using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Count.Stuff.Services
{
    public class SqliteService<T> : ISqliteService<T> where T : new()
    {
        public readonly SQLiteConnection _table;

        public SqliteService()
        {
            _table = DependencyService.Get<ICreateSqliteTable>().Create<T>();
        }

        public void Add(T entity)
        {
            _table.Insert(entity);
        }

        public void AddRange(List<T> entities)
        {
            _table.InsertAll(entities);
        }

        public IEnumerable<T> Get()
        {
            return (from tbl in _table.Table<T>() select tbl).ToList();
        }
    }
}
