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
using System.Text.Json;
using Xchange.Connector.SDK.Client.AppNetwork;

namespace Connector.Datasets.v1.Dataset;

public class DatasetDataReader : TypedAsyncDataReaderBase<DatasetDataObject>
{
    private readonly ILogger<DatasetDataReader> _logger;
    private readonly IPowerBIClient _apiClient;

    public DatasetDataReader(
        ILogger<DatasetDataReader> logger,
        IPowerBIClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<DatasetDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        DatasetDataObject? result = null;
        try
        {
            // Get workspace/group ID from configuration if specified
            var groupId = dataObjectRunArguments?.RequestParameterOverrides?.Deserialize<Dictionary<string, string>>()?.GetValueOrDefault("GroupId");

            DatasetDataObject? response;
            var keyResolver = new DefaultDataObjectKey();
            var datasetId = dataObjectRunArguments != null 
                ? keyResolver.BuildKeyResolver()(dataObjectRunArguments).UrlPart 
                : throw new ArgumentNullException(nameof(dataObjectRunArguments));

            if (!string.IsNullOrEmpty(groupId))
            {
                // Get dataset from specified group
                response = await _apiClient.GetAsync<DatasetDataObject>(
                    $"groups/{groupId}/datasets/{datasetId}",
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                // Get dataset from My workspace
                response = await _apiClient.GetAsync<DatasetDataObject>(
                    $"datasets/{datasetId}",
                    cancellationToken)
                    .ConfigureAwait(false);
            }

            if (response != null)
            {
                result = response;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving dataset");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving dataset");
            throw;
        }
        if (result != null)
        {
            yield return result;
        }
    }
}