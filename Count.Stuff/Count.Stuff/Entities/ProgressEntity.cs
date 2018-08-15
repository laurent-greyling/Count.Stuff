using Microsoft.WindowsAzure.Storage.Table;

namespace Count.Stuff.Entities
{
    public class ProgressEntity: TableEntity
    {
        /// <summary>
        /// Progress for normal search and count
        /// </summary>
        public int NormalProgress { get; set; }

        /// <summary>
        /// Progress of garden search
        /// </summary>
        public int GardenProgress { get; set; }

        /// <summary>
        /// Number of objects from normal search to compare to
        /// </summary>
        public int NumberOfNormalObjects { get; set; }

        /// <summary>
        /// Number of objects from garden search to compare to
        /// </summary>
        public int NumberOfGardenObjects { get; set; }

        /// <summary>
        /// Checks if normal search is done
        /// </summary>
        public bool IsNormalSearchDone { get; set; }

        /// <summary>
        /// Checks if search for gardens is done
        /// </summary>
        public bool IsGardenSearchDone { get; set; }
    }
}
