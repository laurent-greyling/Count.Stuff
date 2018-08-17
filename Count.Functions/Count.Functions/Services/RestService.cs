using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Count.Functions.Services
{
    public class RestService : IRestService
    {
        readonly HttpClient _httpClient;

        public RestService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string request)
        {
            //This was needed as sometimes the api returned XML even if JSON was specified as chosen data type.
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //ConfigureAwait see https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
