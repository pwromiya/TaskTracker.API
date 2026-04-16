using System.Net.Http.Json;
using TaskTracker.Contracts.Responses;
using TaskTracker.Domain.Common;

namespace TaskTracker.Infrastructure.Api;

internal static class ApiErrorParser
{
    public static async Task ThrowFromResponseAsync(HttpResponseMessage response)
    {
        ErrorResponse? errorResponse = null;

        try
        {
            errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        }
        catch
        {
            // Ignore parsing errors and fall back to generic key.
        }

        var translationKey = string.IsNullOrWhiteSpace(errorResponse?.TranslationKey)
            ? "UnexpectedError"
            : errorResponse.TranslationKey;

        throw new AppException(translationKey);
    }
}
