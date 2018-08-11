
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    /// <summary>
    /// Service to Access azure services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IAzureService<T>
    {
        /// <summary>
        /// Insert or merge
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertOrMerge(T entity);

        /// <summary>
        /// Send a message to storage queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessage(string message);
    }
}
