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
using System.Text.Json.Serialization;

namespace Connector.PushDatasets.v1.Dataset.Create;

public class CreateDatasetHandler : IActionHandler<CreateDatasetAction>
{
    private readonly ILogger<CreateDatasetHandler> _logger;
    private readonly IPowerBIClient _powerBIClient;

    public CreateDatasetHandler(
        ILogger<CreateDatasetHandler> logger,
        IPowerBIClient powerBIClient)
    {
        _logger = logger;
        _powerBIClient = powerBIClient;
    }

    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance,
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<CreateDatasetActionInput>(actionInstance.InputJson)
            ?? throw new InvalidOperationException("Input was null");

        try
        {
            // Create request body
            var request = new CreateDatasetRequest
            {
                Name = input.Name,
                DefaultMode = input.DefaultMode,
                Tables = input.Tables
            };

            // Call appropriate API based on whether groupId is provided
            DatasetDataObject response;
            if (string.IsNullOrEmpty(input.GroupId))
            {
                response = await _powerBIClient.PostAsync<DatasetDataObject>(
                    $"v1.0/myorg/datasets{GetRetentionPolicyQueryString(input.DefaultRetentionPolicy)}",
                    request,
                    cancellationToken);
            }
            else
            {
                response = await _powerBIClient.PostAsync<DatasetDataObject>(
                    $"v1.0/myorg/groups/{input.GroupId}/datasets{GetRetentionPolicyQueryString(input.DefaultRetentionPolicy)}",
                    request,
                    cancellationToken);
            }

            // Create output
            var output = new CreateDatasetActionOutput
            {
                Id = response.Id,
                Name = response.Name,
                DefaultRetentionPolicy = response.DefaultRetentionPolicy?.ToString()
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
                new() { DataObjectType = typeof(DatasetDataObject), CacheChanges = operations.ToArray() }
            };

            return ActionHandlerOutcome.Successful(output, resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to create Power BI dataset '{DatasetName}'", input.Name);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "CreateDatasetHandler" },
                        Text = $"Failed to create dataset: {ex.Message}"
                    }
                }
            });
        }
    }

    private static string GetRetentionPolicyQueryString(DefaultRetentionPolicy? policy)
    {
        return policy.HasValue ? $"?defaultRetentionPolicy={policy.Value.ToString().ToLowerInvariant()}" : string.Empty;
    }
}

public class CreateDatasetRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("defaultMode")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatasetMode DefaultMode { get; set; }

    [JsonPropertyName("tables")]
    public List<Table> Tables { get; set; } = new();
}
