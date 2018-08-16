using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

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
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //ConfigureAwait see https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
