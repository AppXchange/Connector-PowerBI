using Connector.Client;
using ESR.Hosting.Action;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Action;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Client.AppNetwork;
using System.Text.Json.Serialization;

namespace Connector.Groups.v1.Group.Create;

public class CreateGroupHandler : IActionHandler<CreateGroupAction>
{
    private readonly ILogger<CreateGroupHandler> _logger;
    private readonly IApiClient _apiClient;

    public CreateGroupHandler(
        ILogger<CreateGroupHandler> logger,
        IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance, 
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<CreateGroupActionInput>(actionInstance.InputJson);
        
        try
        {
            // Build the URL with optional workspaceV2 parameter
            var url = "v1.0/myorg/groups";
            if (input?.WorkspaceV2 == true)
            {
                url += "?workspaceV2=true";
            }

            // Create request body
            var requestBody = new
            {
                name = input?.Name
            };

            // Make POST request to create group
            var response = await _apiClient.PostAsync<GroupResponse>(
                url,
                requestBody,
                cancellationToken);

            if (response?.Value == null || response.Value.Count == 0)
            {
                throw new HttpRequestException("No group was created in the response");
            }

            var createdGroup = response.Value[0];

            // Map response to output
            var output = new CreateGroupActionOutput
            {
                Id = createdGroup.Id,
                Name = createdGroup.Name,
                IsReadOnly = createdGroup.IsReadOnly,
                IsOnDedicatedCapacity = createdGroup.IsOnDedicatedCapacity
            };

            // Build sync operations to update cache
            var operations = new List<SyncOperation>();
            var keyResolver = new DefaultDataObjectKey();
            var key = keyResolver.BuildKeyResolver()(createdGroup);
            
            operations.Add(SyncOperation.CreateSyncOperation(
                UpdateOperation.Upsert.ToString(), 
                key.UrlPart,
                key.PropertyNames, 
                createdGroup));

            var resultList = new List<CacheSyncCollection>
            {
                new() 
                { 
                    DataObjectType = typeof(GroupDataObject), 
                    CacheChanges = operations.ToArray() 
                }
            };

            return ActionHandlerOutcome.Successful(output, resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error creating group with name {GroupName}", input?.Name);

            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = new[] { "CreateGroupHandler" },
                        Text = $"Failed to create group: {ex.Message}"
                    }
                }
            });
        }
    }
}

/// <summary>
/// Response wrapper for group creation
/// </summary>
public class GroupResponse
{
    [JsonPropertyName("value")]
    public List<GroupDataObject> Value { get; init; } = new();
}
