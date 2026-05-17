using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessRegistrationSystem.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://erocapiv2.drc.gov.lk/api/v1/eroc/name/search";
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjBiZjNjZWNjNjQ4MWY3ZWYwZWFlNGZmYzJhMjZjMDMwMWFhYTJjY2U2NWVlMmRiZjdkMjg1NjBjYjZlMTM1ODIyYTQ5MGZiMTdjNDhkYmZiIn0";

        public BusinessService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SearchNameAsync(string searchText)
        {
            try
            {
                var payload = new
                {
                    criteria = 2,
                    searchtext = searchText,
                    token = Token
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                // Handle non-success status codes
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"{{\"error\": \"API returned status {(int)response.StatusCode}\", \"details\": \"{errorContent}\"}}";
            }
            catch (HttpRequestException ex)
            {
                return $"{{\"error\": \"Network error: {ex.Message}\"}}";
            }
            catch (TaskCanceledException)
            {
                return "{\"error\": \"The request timed out.\"}";
            }
            catch (System.Exception ex)
            {
                return $"{{\"error\": \"An unexpected error occurred: {ex.Message}\"}}";
            }
        }
    }
}
