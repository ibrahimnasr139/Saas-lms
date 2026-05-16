using Application.Contracts.Externals;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services
{
    public sealed class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;

        public ExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> CallExternalServiceAsync<TPayload, TResponse>(string endpoint, TPayload payload,
        CancellationToken cancellationToken) where TPayload : class where TResponse : class
        {
            var json = JsonSerializer.Serialize(payload);
            Console.WriteLine($"AI Payload: {json}");

            var response = await _httpClient.PostAsJsonAsync(endpoint, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Status: {response.StatusCode} - Body: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
            if (result is null)
                throw new Exception("Failed to deserialize response");

            return result;
        }
    }
}
