using System.Threading.Tasks;
using System.Net.Http;

namespace Count.Stuff.Services
{
    public class RestCall : IRestCall
    {
        readonly HttpClient _httpClient;

        public RestCall()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string request)
        {
            //ConfigureAwait see https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
