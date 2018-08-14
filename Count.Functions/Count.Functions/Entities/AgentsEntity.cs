using Microsoft.WindowsAzure.Storage.Table;

namespace Count.Functions.Entities
{
    public class AgentsEntity: TableEntity
    {
        /// <summary>
        /// Number of entities for agent in all objects
        /// </summary>
        public int NormalCount { get; set; }

        /// <summary>
        /// Number of entities for agent where garden is involved
        /// </summary>
        public int GardenCount { get; set; }

        public string AgentName { get; set; }
    }
}
