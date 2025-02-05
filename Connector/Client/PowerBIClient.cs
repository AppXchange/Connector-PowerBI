using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Connector.PushDatasets.v1.Dataset;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Connector.Imports.v1.Import.Create;
using Connector.Imports.v1.TempUploadLocation.Create;

namespace Connector.Client;

public class PowerBIClient : IPowerBIClient, IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PowerBIClient> _logger;

    public PowerBIClient(IHttpClientFactory httpClientFactory, ILogger<PowerBIClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PowerBIClient");
        _logger = logger;
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
            _logger.LogError(ex, "Error testing Power BI connection");
            return false;
        }
    }

    async Task<T> IApiClient.GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            throw new ApiException($"GET request failed for URL: {url}", 500, ex.Message);
        }
    }

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new ApiException(
                $"API request failed with status code: {response.StatusCode}",
                (int)response.StatusCode,
                content);
        }
    }

    private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken) 
                ?? throw new ApiException("Response content was null", (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            throw new ApiException("Failed to deserialize response", (int)response.StatusCode, ex.Message);
        }
    }

    async Task<T> IApiClient.PostAsync<T>(string url, object data, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken) ?? default!;
    }

    async Task<T> IApiClient.PutAsync<T>(string url, T data, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync(url, data, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken) ?? default!;
    }

    async Task<T> IApiClient.PatchAsync<T>(string url, object data, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PatchAsJsonAsync(url, data, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken) ?? default!;
    }

    public async Task DeleteAsync<T>(string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<DatasetDataObject>> GetDatasetsAsync(CancellationToken cancellationToken)
    {
        var datasets = await ((IApiClient)this).GetAsync<IEnumerable<DatasetDataObject>>("datasets", cancellationToken);
        return datasets ?? Enumerable.Empty<DatasetDataObject>();
    }

    public async Task<IEnumerable<DatasetDataObject>> GetDatasetsInGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        var datasets = await ((IApiClient)this).GetAsync<IEnumerable<DatasetDataObject>>($"groups/{groupId}/datasets", cancellationToken);
        return datasets ?? Enumerable.Empty<DatasetDataObject>();
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
                System.Net.HttpStatusCode.InternalServerError, 
                "Failed to create import: " + ex.Message);
        }
    }

    public async Task<ApiResponse<T>> CreateImport<T>(CreateTempUploadLocationActionInput input, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("imports/createTemporaryUploadLocation", input, cancellationToken);
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
                System.Net.HttpStatusCode.InternalServerError, 
                "Failed to create temporary upload location: " + ex.Message);
        }
    }
} 