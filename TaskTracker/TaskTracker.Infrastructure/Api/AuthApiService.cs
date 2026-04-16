using System.Net.Http;
using System.Net.Http.Json;
using TaskTracker.Contracts.Responses;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Application.Services;
namespace TaskTracker.Infrastructure.Api;

/// <inheritdoc cref="IAuthApiService" />
public class AuthApiService: IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;

    public AuthApiService(HttpClient httpClient, TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    /// <inheritdoc />
    public async Task<RegisterResponse?> Register(string login, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
        {
            login,
            password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<RegisterResponse>();
    }

    /// <inheritdoc />
    public async Task<LoginResponse?> Login(string login, string password)
    {

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
        {
            login,
            password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _tokenStorage.Token = result.Token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);

        return result;
    }

    /// <inheritdoc />
    public void ClearToken()
    {
        _tokenStorage.Token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}