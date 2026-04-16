using System.Net.Http;
using System.Net.Http.Json;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Contracts.Requests;
using TaskTracker.Application.Services;

namespace TaskTracker.Infrastructure.Api;

/// <inheritdoc cref="ITaskApiService" />
public class TaskApiService : ITaskApiService
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;
    public TaskApiService(HttpClient httpClient, TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    /// <inheritdoc />
    public async Task CreateTaskAsync(string title, string? description, int projectId, int status)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);

        var request = new CreateTaskRequest
        {
            Title = title,
            Description = description,
            ProjectId = projectId,
            Status = status
        };

        var response = await _httpClient.PostAsJsonAsync("api/tasks", request);
        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
    }

    /// <inheritdoc />
    public async Task UpdateTaskAsync(int id, string title, string? description, int status)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
       new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.PutAsJsonAsync($"api/tasks/{id}", new
        {
            title,
            description,
            status
        });

        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
    }

    /// <inheritdoc />
    public async Task DeleteTaskAsync(int id)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
    }

    /// <inheritdoc />
    public async Task<List<TaskDto>> GetByProjectIdAsync(int projectId)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
       new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStorage.Token);
        var response = await _httpClient.GetAsync($"api/tasks/project/{projectId}");
        if (!response.IsSuccessStatusCode)
            await ApiErrorParser.ThrowFromResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<TaskDto>>();
    }
}
