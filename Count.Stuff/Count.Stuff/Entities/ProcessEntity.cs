using SQLite;

namespace Count.Stuff.Entities
{
    /// <summary>
    /// Entity for keeping the process ID
    /// This ID is to Identify the process the function creates
    /// </summary>
    public class ProcessEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string ProcessId { get; set; }
    }
}
