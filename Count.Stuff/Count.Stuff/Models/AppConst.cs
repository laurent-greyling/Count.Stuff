namespace Count.Stuff.Models
{
    /// <summary>
    /// Class to keep some constant strings around, commonly used throughout the app
    /// </summary>
    public class AppConst
    {
        /// <summary>
        /// Storage connection string. This is here as I have no solution yet to have it in a config file rather
        /// </summary>
        public const string ConnectionString = "";

        /// <summary>
        /// Queue to read and send messages for workers to process
        /// </summary>
        public const string ManagementQueueName = "management";

        /// <summary>
        /// Table to log the progress of the process for client to know if it can continue
        /// </summary>
        public const string ProgressTable = "progress";

        public const string CountProgressPartitionKey = "CountProgress";

        /// <summary>
        /// Table to log the number of times an agent is present in search
        /// </summary>
        public const string AgentsTable = "agents";
    }
}
