using System.Threading.Tasks;

namespace Count.Functions.Services
{
    public interface IRestService
    {
        /// <summary>
        /// Does Get call to the API service and return Json string to be deserialized 
        /// This will do nothing more that fetch the data requested, for manipulation of data this need to be passed 
        /// to class/method that will use this data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<string> GetAsync(string request);
    }
}
