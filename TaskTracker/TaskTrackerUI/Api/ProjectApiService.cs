using Azure;
using System.Net.Http;
using System.Net.Http.Json;
using TaskTracker.Contracts.Requests;
using TaskTracker.UI.Api;
using TaskTracker.UI.Services;

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
        return await _httpClient.GetFromJsonAsync<List<ProjectDto>>("api/projects");
    }

    /// <inheritdoc />
    public async Task<ProjectDto> CreateProjectAsync(string name, string description)
    {
        var response = await _httpClient.PostAsJsonAsync("api/projects", new
        {
            name,
            description
        });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProjectDto>();
    }

    /// <inheritdoc />
    public async Task<List<ProjectDto>> GetUserProjectsAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.GetAsync("api/projects");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
    }

    /// <inheritdoc />
    public async Task DeleteProjectAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task UpdateProjectAsync(int id, string name, string? description)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/projects/{id}", new
        {
            name,
            description
        });

        response.EnsureSuccessStatusCode();
    }
}