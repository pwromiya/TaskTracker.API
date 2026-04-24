using System.Net.Http;
using System.Net.Http.Json;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Application.Services;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.Infrastructure.Api;

/// <inheritdoc cref="IProjectApiService" />
public class ProjectApiService : IProjectApiService
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;

    public ProjectApiService(HttpClient httpClient, TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    /// <inheritdoc />
    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.GetAsync("api/projects");

        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<List<ProjectDto>>() ?? new();
    }

    /// <inheritdoc />
    public async Task<ProjectDto> CreateProjectAsync(string name, string description)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.PostAsJsonAsync("api/projects", new CreateProjectRequest
        {
            Name = name,
            Description = description
        });

        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<ProjectDto>();
    }

    /// <inheritdoc />
    public async Task<List<ProjectDto>> GetUserProjectsAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.GetAsync("api/projects");

        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
    }

    /// <inheritdoc />
    public async Task DeleteProjectAsync(int id)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.DeleteAsync($"api/projects/{id}");
        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
    }

    /// <inheritdoc />
    public async Task UpdateProjectAsync(int id, string name, string? description)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.PutAsJsonAsync($"api/projects/{id}", new
        {
            name,
            description
        });

        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
    }
}