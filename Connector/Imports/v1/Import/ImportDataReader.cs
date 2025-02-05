using Connector.Client;
using System;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Xchange.Connector.SDK.CacheWriter;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Connector.Imports.v1.Import;

public class ImportDataReader : TypedAsyncDataReaderBase<ImportDataObject>
{
    private readonly ILogger<ImportDataReader> _logger;
    private readonly IApiClient _apiClient;

    public ImportDataReader(
        ILogger<ImportDataReader> logger,
        IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<ImportDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (dataObjectRunArguments?.RequestParameterOverrides == null)
        {
            throw new ArgumentNullException(nameof(dataObjectRunArguments));
        }

        ImportDataObject result;
        try
        {
            var parameters = dataObjectRunArguments.RequestParameterOverrides.Deserialize<Dictionary<string, string>>();
            var groupId = parameters?.GetValueOrDefault("groupId");
            var importId = parameters?.GetValueOrDefault("importId") 
                ?? throw new ArgumentNullException("importId");

            result = !string.IsNullOrEmpty(groupId)
                ? await GetGroupImport(groupId, importId, cancellationToken)
                : await GetImport(importId, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Exception while making a read request to get import data");
            throw;
        }

        if (result != null)
        {
            yield return result;
        }
    }

    private async Task<ImportDataObject> GetImport(string importId, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetAsync<ApiResponse<ImportDataObject>>(
            $"v1.0/myorg/imports/{importId}",
            cancellationToken);
        
        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to retrieve import. API StatusCode: {response.StatusCode}");
        }
        return response.Data ?? throw new Exception("Import data was null");
    }

    private async Task<ImportDataObject> GetGroupImport(string groupId, string importId, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetAsync<ApiResponse<ImportDataObject>>(
            $"v1.0/myorg/groups/{groupId}/imports/{importId}",
            cancellationToken);
            
        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to retrieve group import. API StatusCode: {response.StatusCode}");
        }
        return response.Data ?? throw new Exception("Import data was null");
    }

    public async Task<ApiResponse<ImportDataObject>> GetImportById(string id, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetAsync<ApiResponse<ImportDataObject>>($"imports/{id}", cancellationToken);
        return response;
    }

    public async Task<ApiResponse<ImportDataObject>> GetImportByGroupId(string groupId, string id, CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetAsync<ApiResponse<ImportDataObject>>($"groups/{groupId}/imports/{id}", cancellationToken);
        return response;
    }
}