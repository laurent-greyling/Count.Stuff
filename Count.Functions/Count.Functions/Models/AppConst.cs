namespace Count.Functions.Models
{
    public class AppConst
    {
        /// <summary>
        /// This is bit silly, but to keep it simple, will use what was given.
        /// Would prefer to give user defined input
        /// </summary>
        public const string SearchQuery = "/?type=koop&zo=/amsterdam/";

        /// <summary>
        /// Queue to read and send messages for workers to process
        /// </summary>
        public const string ManagementQueueName = "management";
    }
}
