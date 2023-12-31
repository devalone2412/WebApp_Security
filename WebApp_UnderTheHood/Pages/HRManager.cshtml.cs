using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood;

namespace MyApp.Namespace
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty] public List<WeatherForecast> WeatherForecasts { get; set; } = [];

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            var res = await httpClient.PostAsJsonAsync("auth",
                new Credential { UserName = "admin", Password = "password" });
            var streamJwt = await res.Content.ReadAsStreamAsync();
            var token = await JsonSerializer.DeserializeAsync<JwtToken>(streamJwt);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            WeatherForecasts = await httpClient.GetFromJsonAsync<List<WeatherForecast>>("weatherforecast") ?? [];
        }
    }

    public class JwtToken
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("expires_at")] public DateTime ExpiresAt { get; set; }
    }
}