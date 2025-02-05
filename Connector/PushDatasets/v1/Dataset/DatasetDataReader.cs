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

namespace Connector.PushDatasets.v1.Dataset;

public class DatasetDataReader : TypedAsyncDataReaderBase<DatasetDataObject>
{
    private readonly ILogger<DatasetDataReader> _logger;
    private readonly IPowerBIClient _powerBIClient;
    private readonly string? _groupId;

    public DatasetDataReader(
        ILogger<DatasetDataReader> logger,
        IPowerBIClient powerBIClient,
        string? groupId = null)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
        _groupId = groupId;
    }

    public override async IAsyncEnumerable<DatasetDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IEnumerable<DatasetDataObject> datasets;
        
        try
        {
            datasets = string.IsNullOrEmpty(_groupId) 
                ? await _powerBIClient.GetDatasetsAsync(cancellationToken)
                : await _powerBIClient.GetDatasetsInGroupAsync(_groupId, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve datasets from Power BI{GroupInfo}", 
                _groupId == null ? "" : $" group {_groupId}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving datasets from Power BI{GroupInfo}",
                _groupId == null ? "" : $" group {_groupId}");
            throw;
        }

        foreach (var dataset in datasets)
        {
            if (dataset.DefaultMode == DatasetMode.Push || 
                dataset.DefaultMode == DatasetMode.PushStreaming)
            {
                yield return dataset;
            }
        }
    }
}