using Connector.Client;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Xchange.Connector.SDK.CacheWriter;

namespace Connector.PushDatasets.v1.Tables;

public class TablesDataReader : TypedAsyncDataReaderBase<TablesDataObject>
{
    private readonly ILogger<TablesDataReader> _logger;
    private readonly IPowerBIClient _powerBIClient;
    private readonly string? _groupId;
    private readonly string _datasetId;

    public TablesDataReader(
        ILogger<TablesDataReader> logger,
        IPowerBIClient powerBIClient,
        string datasetId,
        string? groupId = null)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
        _datasetId = datasetId;
        _groupId = groupId;
    }

    public override async IAsyncEnumerable<TablesDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IEnumerable<TablesDataObject> tables;
        try
        {
            tables = string.IsNullOrEmpty(_groupId)
                ? await _powerBIClient.GetAsync<IEnumerable<TablesDataObject>>(
                    $"v1.0/myorg/datasets/{_datasetId}/tables",
                    cancellationToken)
                : await _powerBIClient.GetAsync<IEnumerable<TablesDataObject>>(
                    $"v1.0/myorg/groups/{_groupId}/datasets/{_datasetId}/tables",
                    cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve tables from Power BI dataset '{DatasetId}'{GroupInfo}",
                _datasetId, _groupId == null ? "" : $" in group {_groupId}");
            throw;
        }

        foreach (var table in tables)
        {
            var tableWithContext = new TablesDataObject
            {
                Name = table.Name,
                Columns = table.Columns,
                Measures = table.Measures,
                Description = table.Description,
                IsHidden = table.IsHidden,
                DatasetId = _datasetId,
                GroupId = _groupId
            };

            yield return tableWithContext;
        }
    }
}