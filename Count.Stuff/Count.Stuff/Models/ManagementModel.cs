
namespace Count.Stuff.Models
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
        /// Current object we need to process
        /// </summary>
        public int ObjectNumber { get; set; }
    }
}
