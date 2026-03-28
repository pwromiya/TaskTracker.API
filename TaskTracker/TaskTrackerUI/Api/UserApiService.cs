using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TaskTracker.Contracts.Responses;
using TaskTracker.UI.Services;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.Api;

/// <inheritdoc cref="IUserApiService" />
public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;
    public UserApiService(HttpClient httpClient, TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    /// <inheritdoc />
    public async Task<int> GetCurrentUserIdAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.GetAsync("api/user/me");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UserMeResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result!.Id;
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(string newPassword)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var request = new ChangePasswordRequest
        {
            NewPassword = newPassword
        };

        var response = await _httpClient.PostAsJsonAsync("api/user/change-password", request);

        response.EnsureSuccessStatusCode();
    }
}
