using Application.Constants;
using Application.Contracts.Zoom;
using Application.Features.Tenants.Commands.CreateLiveSession;
using Application.Features.Tenants.Commands.UpdateLiveSession;
using Application.Features.ZoomIntegration.Dtos;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    internal class ZoomService : IZoomService
    {
        private readonly IOptions<ZoomOptions> _zoomOptions;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public ZoomService(IOptions<ZoomOptions> options, HttpClient httpClient, IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory)
        {
            _zoomOptions = options;
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }

        public string GetAuthorizationUrl(string state, CancellationToken cancellationToken) =>
            $"{ZoomConstants.AuthorizationUrl}?" +
            $"response_type=code&" +
            $"client_id={Uri.EscapeDataString(_zoomOptions.Value.ClientId)}&" +
            $"redirect_uri={Uri.EscapeDataString(_zoomOptions.Value.RedirectUri)}&" +
            $"state={state}";
        public async Task<ZoomTokenResponse?> ExchangeCodeToTokenAsync(string code, string state, CancellationToken cancellationToken)
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>(ZoomConstants.Code, code),
                new KeyValuePair<string, string>(ZoomConstants.RedirectUri, _zoomOptions.Value.RedirectUri)
            });

            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_zoomOptions.Value.ClientId}:{_zoomOptions.Value.ClientSecret}"));

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, ZoomConstants.TokenUrl)
            {
                Content = requestBody
            };
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(tokenRequest, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<ZoomTokenResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<ZoomUserResponse?> GetZoomUserInfoAsync(string accessToken, CancellationToken cancellationToken)
        {
            var userHttpClient = _httpClientFactory.CreateClient();
            userHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ZoomConstants.Bearer, accessToken);
            var userResponse = await userHttpClient.GetAsync(ZoomConstants.ZoomUserMe, cancellationToken);

            var responseContent = await userResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!userResponse.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<ZoomUserResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<bool> RefreshZoomTokenAsync(ZoomIntegration integration, CancellationToken cancellationToken)
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", integration.RefreshToken)
            });

            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_zoomOptions.Value.ClientId}:{_zoomOptions.Value.ClientSecret}"));

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, ZoomConstants.TokenUrl)
            {
                Content = requestBody
            };
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(tokenRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenData = JsonSerializer.Deserialize<ZoomTokenResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenData == null)
                return false;

            integration.AccessToken = tokenData.access_token;
            integration.RefreshToken = tokenData.refresh_token;
            integration.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.expires_in);

            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
        public async Task<ZoomMeetingResponse?> CreateZoomMeetingAsync(string accessToken, CreateLiveSessionCommand request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var meetingRequest = new
            {
                topic = request.Title,
                agenda = request.Description,
                start_time = request.ScheduledAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = request.Duration,
                type = 2,
                timezone = "UTC",
                settings = new
                {
                    participant_video = request.Settings.ParticipantVideo,
                    allow_chat = request.Settings.EnableChat,
                    private_chat = request.Settings.EnableChat,
                    waiting_room = request.Settings.WaitingRoom,
                    host_video = true,
                    join_before_host = false,
                    mute_upon_entry = true,
                    auto_recording = "cloud",
                    allow_multiple_devices = true,
                    approval_type = 1,
                    audio = "both",
                    meeting_authentication = false
                }
            };

            var response = await client.PostAsJsonAsync(ZoomConstants.MeetingRequest, meetingRequest, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<ZoomMeetingResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<bool> UpdateZoomMeetingAsync(string accessToken, string meetingId, UpdateLiveSessionCommand request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var updateRequest = new
            {
                topic = request.Title,
                agenda = request.Description,
                start_time = request.ScheduledAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = request.Duration,
                timezone = "UTC",
                settings = new
                {
                    participant_video = request.Settings.ParticipantVideo,
                    allow_chat = request.Settings.EnableChat,
                    private_chat = request.Settings.EnableChat,
                    waiting_room = request.Settings.WaitingRoom,
                    host_video = true,
                    join_before_host = false,
                    mute_upon_entry = true,
                    auto_recording = "none",
                    allow_multiple_devices = true,
                    approval_type = 1,
                    audio = "both",
                    meeting_authentication = false
                }
            };

            var response = await client.PatchAsJsonAsync($"{ZoomConstants.ZoomMeetingsUrl}{meetingId}", updateRequest, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteZoomMeetingAsync(string accessToken, string meetingId, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.DeleteAsync($"{ZoomConstants.ZoomMeetingsUrl}{meetingId}", cancellationToken);

            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }
    }
}