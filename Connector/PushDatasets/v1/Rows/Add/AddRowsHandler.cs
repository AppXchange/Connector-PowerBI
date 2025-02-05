using Connector.Client;
using ESR.Hosting.Action;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Action;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Client.AppNetwork;

namespace Connector.PushDatasets.v1.Rows.Add;

public class AddRowsHandler : IActionHandler<AddRowsAction>
{
    private readonly ILogger<AddRowsHandler> _logger;
    private readonly IPowerBIClient _powerBIClient;

    public AddRowsHandler(
        ILogger<AddRowsHandler> logger,
        IPowerBIClient powerBIClient)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance,
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<AddRowsActionInput>(actionInstance.InputJson)
            ?? throw new InvalidOperationException("Input was null");

        try
        {
            // Create request body
            var request = new { rows = input.Rows };

            // Call appropriate API based on whether groupId is provided
            if (string.IsNullOrEmpty(input.GroupId))
            {
                await _powerBIClient.PostAsync<object>(
                    $"v1.0/myorg/datasets/{input.DatasetId}/tables/{input.TableName}/rows",
                    request,
                    cancellationToken);
            }
            else
            {
                await _powerBIClient.PostAsync<object>(
                    $"v1.0/myorg/groups/{input.GroupId}/datasets/{input.DatasetId}/tables/{input.TableName}/rows",
                    request,
                    cancellationToken);
            }

            var output = new AddRowsActionOutput
            {
                Success = true,
                RowCount = input.Rows.Count
            };

            // Create a RowsDataObject for caching
            var rowsDataObject = new RowsDataObject
            {
                Id = Guid.NewGuid().ToString(),
                DatasetId = input.DatasetId,
                TableName = input.TableName,
                GroupId = input.GroupId,
                Rows = input.Rows
            };

            // Build sync operations to update cache
            var operations = new List<SyncOperation>();
            var keyResolver = new DefaultDataObjectKey();
            var key = keyResolver.BuildKeyResolver()(rowsDataObject);
            operations.Add(SyncOperation.CreateSyncOperation(
                UpdateOperation.Upsert.ToString(),
                key.UrlPart,
                key.PropertyNames,
                rowsDataObject));

            var resultList = new List<CacheSyncCollection>
            {
                new() { DataObjectType = typeof(RowsDataObject), CacheChanges = operations.ToArray() }
            };

            return ActionHandlerOutcome.Successful(output, resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to add rows to Power BI dataset '{DatasetId}' table '{TableName}'", 
                input.DatasetId, input.TableName);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "AddRowsHandler" },
                        Text = $"Failed to add rows: {ex.Message}"
                    }
                }
            });
        }
    }
}
