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
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Connector.Datasets.v1.Datasets;

public class DatasetsDataReader : TypedAsyncDataReaderBase<DatasetsDataObject>
{
    private readonly ILogger<DatasetsDataReader> _logger;
    private readonly IPowerBIClient _apiClient;

    public DatasetsDataReader(
        ILogger<DatasetsDataReader> logger,
        IPowerBIClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<DatasetsDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        DatasetsList? response = null;
        try
        {
            // Get workspace/group ID from configuration if specified
            var groupId = dataObjectRunArguments?.RequestParameterOverrides?.Deserialize<Dictionary<string, string>>()?.GetValueOrDefault("GroupId");

            // Define the API endpoint based on whether we have a group ID
            var endpoint = !string.IsNullOrEmpty(groupId) 
                ? $"groups/{groupId}/datasets"
                : "datasets";

            // Get datasets list from Power BI API
            response = await _apiClient.GetAsync<DatasetsList>(endpoint, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving datasets");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving datasets");
            throw;
        }
        if (response?.Value != null)
        {
            foreach (var dataset in response.Value)
            {
                yield return dataset;
            }
        }
    }

    private class DatasetsList
    {
        [JsonPropertyName("value")]
        public List<DatasetsDataObject> Value { get; init; } = new();
    }
}