
namespace Count.Functions.Models
{
    /// <summary>
    /// Model holding information to manage the message sent and handled to and from the queue
    /// </summary>
    public class ManagementModel
    {
        /// <summary>
        /// Assigned ID from client, preferably a GUID
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// Message Type to direct messagehandler to correct process
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Number of objects we need to work through
        /// </summary>
        public int NumberOfObjects { get; set; }

        /// <summary>
        /// Current object we need to process
        /// </summary>
        public int ObjectNumber { get; set; }

        public int NumberOfPages { get; set; }

        /// <summary>
        /// Is the search based on normal or with criteria garden added
        /// </summary>
        public bool IsGardenSearch { get; set; }
    }
}
