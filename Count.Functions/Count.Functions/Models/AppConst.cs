namespace Count.Functions.Models
{
    /// <summary>
    /// Class to keep some constant strings around, commonly used throughout the app
    /// </summary>
    public class AppConst
    {
        /// <summary>
        /// Would prefer to give user defined input
        /// </summary>
        public const string SearchQuery = "/?type=koop&zo=/amsterdam/";

        /// <summary>
        /// Queue to read and send messages for workers to process
        /// </summary>
        public const string ManagementQueueName = "management";

        /// <summary>
        /// Table to log the progress of the process for client to know if it can continue
        /// </summary>
        public const string ProgressTable = "progress";

        /// <summary>
        /// Table to log the number of times an agent is present in search
        /// </summary>
        public const string AgentsTable = "agents";

        public const string CountProgressPartitionKey = "CountProgress";
    }
}
