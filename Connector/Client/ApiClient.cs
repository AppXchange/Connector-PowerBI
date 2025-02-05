using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Connector.Groups.v1.Groups;
using Connector.Imports.v1.Import.Create;
using Connector.Imports.v1.Imports;
using Connector.Imports.v1.TempUploadLocation.Create;
using Connector.PushDatasets.v1.Dataset;
using Microsoft.Extensions.Logging;

namespace Connector.Client;

public interface IApiClient
{
    Task<T> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken);
    Task<T> PostAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken);
    Task<T> PatchAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken);
    Task<T> PutAsync<T>(string url, T data, CancellationToken cancellationToken);
    Task<ApiResponse<T>> CreateImport<T>(CreateImportActionInput input, CancellationToken cancellationToken);
    Task<ApiResponse<T>> CreateImport<T>(CreateTempUploadLocationActionInput input, CancellationToken cancellationToken);
}

public interface IPowerBIClient : IApiClient
{
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
    Task DeleteAsync<T>(string relativeUrl, CancellationToken cancellationToken);
    Task<IEnumerable<DatasetDataObject>> GetDatasetsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<DatasetDataObject>> GetDatasetsInGroupAsync(string groupId, CancellationToken cancellationToken);
}

/// <summary>
/// A client for interfacing with the Power BI API via HTTP
/// </summary>
public class ApiClient : IApiClient, IPowerBIClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, string baseUrl, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedResponse<T>>> GetRecords<T>(string relativeUrl, int page, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{relativeUrl}?page={page}", cancellationToken);
            var apiResponse = new ApiResponse<PaginatedResponse<T>>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                CorrelationId = GetCorrelationId(response),
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken)
            };

            if (response.IsSuccessStatusCode)
            {
                apiResponse.Data = await response.Content.ReadFromJsonAsync<PaginatedResponse<T>>(cancellationToken: cancellationToken);
            }
            else
            {
                apiResponse.ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            }

            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get paginated records from {Url}", relativeUrl);
            throw;
        }
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("v1.0/myorg/groups", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test connection failed");
            return false;
        }
    }

    public async Task<T> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "GET request failed for {Url}", relativeUrl);
            throw await ApiException.FromResponseAsync(await _httpClient.GetAsync(relativeUrl, cancellationToken));
        }
    }

    public async Task<T> PostAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(relativeUrl, body, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "POST request failed for {Url}", relativeUrl);
            throw await ApiException.FromResponseAsync(await _httpClient.PostAsJsonAsync(relativeUrl, body, cancellationToken));
        }
    }

    public async Task<T> PutAsync<T>(string relativeUrl, T data, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(relativeUrl, data, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "PUT request failed for {Url}", relativeUrl);
            throw await ApiException.FromResponseAsync(await _httpClient.PutAsJsonAsync(relativeUrl, data, cancellationToken));
        }
    }

    public async Task<T> PatchAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync(relativeUrl, body, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "PATCH request failed for {Url}", relativeUrl);
            throw await ApiException.FromResponseAsync(await _httpClient.PatchAsJsonAsync(relativeUrl, body, cancellationToken));
        }
    }

    public async Task DeleteAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(relativeUrl, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "DELETE request failed for {Url}", relativeUrl);
            throw await ApiException.FromResponseAsync(await _httpClient.DeleteAsync(relativeUrl, cancellationToken));
        }
    }

    public async Task<IEnumerable<DatasetDataObject>> GetDatasetsAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<IEnumerable<DatasetDataObject>>("v1.0/myorg/datasets", cancellationToken);
    }

    public async Task<IEnumerable<DatasetDataObject>> GetDatasetsInGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        return await GetAsync<IEnumerable<DatasetDataObject>>($"v1.0/myorg/groups/{groupId}/datasets", cancellationToken);
    }

    public async Task<ApiResponse<GroupsResponse>> GetGroups(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("groups", cancellationToken);
            return new ApiResponse<GroupsResponse>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Data = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<GroupsResponse>(cancellationToken: cancellationToken)
                    : null,
                ErrorMessage = !response.IsSuccessStatusCode 
                    ? await response.Content.ReadAsStringAsync(cancellationToken)
                    : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get groups");
            return ApiResponse<GroupsResponse>.Error(
                HttpStatusCode.InternalServerError, 
                "Failed to get groups: " + ex.Message);
        }
    }

    public async Task<ApiResponse<ImportsResponse>> GetImports(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("imports", cancellationToken);
            return new ApiResponse<ImportsResponse>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Data = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<ImportsResponse>(cancellationToken: cancellationToken)
                    : null,
                ErrorMessage = !response.IsSuccessStatusCode 
                    ? await response.Content.ReadAsStringAsync(cancellationToken)
                    : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get imports");
            return ApiResponse<ImportsResponse>.Error(
                HttpStatusCode.InternalServerError, 
                "Failed to get imports: " + ex.Message);
        }
    }

    public async Task<ApiResponse<T>> CreateImport<T>(CreateImportActionInput input, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("imports", input, cancellationToken);
            return new ApiResponse<T>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Data = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
                    : default,
                ErrorMessage = !response.IsSuccessStatusCode 
                    ? await response.Content.ReadAsStringAsync(cancellationToken)
                    : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create import");
            return ApiResponse<T>.Error(
                HttpStatusCode.InternalServerError, 
                "Failed to create import: " + ex.Message);
        }
    }

    public async Task<ApiResponse<T>> CreateImport<T>(CreateTempUploadLocationActionInput input, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("v1.0/myorg/imports/createTemporaryUploadLocation", input, cancellationToken);
            return new ApiResponse<T>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Data = response.IsSuccessStatusCode 
                    ? await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
                    : default,
                ErrorMessage = !response.IsSuccessStatusCode 
                    ? await response.Content.ReadAsStringAsync(cancellationToken)
                    : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create temporary upload location");
            return ApiResponse<T>.Error(
                HttpStatusCode.InternalServerError, 
                "Failed to create temporary upload location: " + ex.Message);
        }
    }

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.FromResponseAsync(response);
        }
    }

    private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken) 
                ?? throw new ApiException("Response content was null", HttpStatusCode.InternalServerError);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw new ApiException("Failed to deserialize response", HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private string? GetCorrelationId(HttpResponseMessage response)
    {
        return response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;
    }
}