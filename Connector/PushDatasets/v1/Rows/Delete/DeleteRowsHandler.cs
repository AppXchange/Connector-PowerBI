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

namespace Connector.PushDatasets.v1.Rows.Delete;

public class DeleteRowsHandler : IActionHandler<DeleteRowsAction>
{
    private readonly ILogger<DeleteRowsHandler> _logger;
    private readonly IPowerBIClient _powerBIClient;

    public DeleteRowsHandler(
        ILogger<DeleteRowsHandler> logger,
        IPowerBIClient powerBIClient)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance,
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<DeleteRowsActionInput>(actionInstance.InputJson)
            ?? throw new InvalidOperationException("Input was null");

        try
        {
            // Call appropriate API based on whether groupId is provided
            if (string.IsNullOrEmpty(input.GroupId))
            {
                await _powerBIClient.DeleteAsync<object>(
                    $"v1.0/myorg/datasets/{input.DatasetId}/tables/{input.TableName}/rows",
                    cancellationToken);
            }
            else
            {
                await _powerBIClient.DeleteAsync<object>(
                    $"v1.0/myorg/groups/{input.GroupId}/datasets/{input.DatasetId}/tables/{input.TableName}/rows",
                    cancellationToken);
            }

            var output = new DeleteRowsActionOutput
            {
                Success = true
            };

            // Create a RowsDataObject for caching with empty rows
            var rowsDataObject = new RowsDataObject
            {
                Id = Guid.NewGuid().ToString(),
                DatasetId = input.DatasetId,
                TableName = input.TableName,
                GroupId = input.GroupId,
                Rows = new() // Empty rows since we deleted them all
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
            _logger.LogError(ex, "Failed to delete rows from Power BI dataset '{DatasetId}' table '{TableName}'", 
                input.DatasetId, input.TableName);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "DeleteRowsHandler" },
                        Text = $"Failed to delete rows: {ex.Message}"
                    }
                }
            });
        }
    }
}
