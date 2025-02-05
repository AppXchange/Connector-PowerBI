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

namespace Connector.PushDatasets.v1.Table.Update;

public class UpdateTableHandler : IActionHandler<UpdateTableAction>
{
    private readonly ILogger<UpdateTableHandler> _logger;
    private readonly IPowerBIClient _powerBIClient;

    public UpdateTableHandler(
        ILogger<UpdateTableHandler> logger,
        IPowerBIClient powerBIClient)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance,
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<UpdateTableActionInput>(actionInstance.InputJson)
            ?? throw new InvalidOperationException("Input was null");

        try
        {
            // Create request body
            var request = new TableDataObject
            {
                Name = input.Name,
                Columns = input.Columns,
                Measures = input.Measures,
                Description = input.Description,
                IsHidden = input.IsHidden
            };

            // Call appropriate API based on whether groupId is provided
            TableDataObject response;
            if (string.IsNullOrEmpty(input.GroupId))
            {
                response = await _powerBIClient.PutAsync<TableDataObject>(
                    $"v1.0/myorg/datasets/{input.DatasetId}/tables/{input.Name}",
                    request,
                    cancellationToken);
            }
            else
            {
                response = await _powerBIClient.PutAsync<TableDataObject>(
                    $"v1.0/myorg/groups/{input.GroupId}/datasets/{input.DatasetId}/tables/{input.Name}",
                    request,
                    cancellationToken);
            }

            var output = new UpdateTableActionOutput
            {
                Name = response.Name
            };

            // Build sync operations to update cache
            var operations = new List<SyncOperation>();
            var keyResolver = new DefaultDataObjectKey();
            var key = keyResolver.BuildKeyResolver()(response);
            operations.Add(SyncOperation.CreateSyncOperation(
                UpdateOperation.Upsert.ToString(),
                key.UrlPart,
                key.PropertyNames,
                response));

            var resultList = new List<CacheSyncCollection>
            {
                new() { DataObjectType = typeof(TableDataObject), CacheChanges = operations.ToArray() }
            };

            return ActionHandlerOutcome.Successful(output, resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to update Power BI table '{TableName}' in dataset '{DatasetId}'", 
                input.Name, input.DatasetId);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "UpdateTableHandler" },
                        Text = $"Failed to update table: {ex.Message}"
                    }
                }
            });
        }
    }
}
