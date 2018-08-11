using Count.Functions.Models;
using System.Threading.Tasks;

namespace Count.Functions.StartUpProcess
{
    public interface IStartUp
    {
        /// <summary>
        /// Run process
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task RunAsync(ManagementModel message);
    }
}
