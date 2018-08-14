namespace Count.Stuff.Models
{
    public class AppConst
    {
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
    }
}
