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

namespace Connector.Datasets.v1.Dataset.Update;

public class UpdateDatasetHandler : IActionHandler<UpdateDatasetAction>
{
    private readonly ILogger<UpdateDatasetHandler> _logger;
    private readonly IPowerBIClient _apiClient;

    public UpdateDatasetHandler(
        ILogger<UpdateDatasetHandler> logger,
        IPowerBIClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<UpdateDatasetActionInput>(actionInstance.InputJson);
        if (input == null)
        {
            throw new ArgumentException("Action input cannot be null", nameof(actionInstance));
        }

        try
        {
            // Build update request body
            var updateRequest = new
            {
                targetStorageMode = input.TargetStorageMode,
                queryScaleOutSettings = input.QueryScaleOutSettings
            };

            // Determine endpoint based on whether GroupId is provided
            string endpoint = !string.IsNullOrEmpty(input.GroupId)
                ? $"groups/{input.GroupId}/datasets/{input.Id}"
                : $"datasets/{input.Id}";

            // Call Power BI API to update dataset
            await _apiClient.PatchAsync<object>(endpoint, updateRequest, cancellationToken)
                .ConfigureAwait(false);

            // Get updated dataset to return and sync
            var response = await _apiClient.GetAsync<DatasetDataObject>(endpoint, cancellationToken)
                .ConfigureAwait(false);

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
                new() { 
                    DataObjectType = typeof(DatasetDataObject), 
                    CacheChanges = operations.ToArray() 
                }
            };

            // Return successful outcome with updated dataset
            return ActionHandlerOutcome.Successful(
                new UpdateDatasetActionOutput { Id = input.Id }, 
                resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while updating dataset {DatasetId}", input.Id);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "UpdateDatasetHandler" },
                        Text = $"Failed to update dataset: {ex.Message}"
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating dataset {DatasetId}", input.Id);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "UpdateDatasetHandler" },
                        Text = $"Unexpected error updating dataset: {ex.Message}"
                    }
                }
            });
        }
    }
}
