namespace Count.Stuff.Models
{
    public class AppConst
    {
        public const string ConnectionString = "myconnection to azure";

        /// <summary>
        /// Queue to read and send messages for workers to process
        /// </summary>
        public const string ManagementQueueName = "management";
    }
}
