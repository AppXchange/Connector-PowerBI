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
using System.Threading.Tasks;

namespace Connector.Groups.v1.Group;

public class GroupDataReader : TypedAsyncDataReaderBase<GroupDataObject>
{
    private readonly ILogger<GroupDataReader> _logger;
    private readonly IApiClient _apiClient;

    public GroupDataReader(
        ILogger<GroupDataReader> logger,
        IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<GroupDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var groupsResponse = await _apiClient.GetAsync<GroupsResponse>(
            "v1.0/myorg/groups",
            cancellationToken);

        foreach (var group in groupsResponse.Value)
        {
            var detailedGroup = await GetDetailedGroupAsync(group.Id, cancellationToken);
            if (detailedGroup != null)
            {
                yield return detailedGroup;
            }
        }
    }

    private async Task<GroupDataObject?> GetDetailedGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        try
        {
            return await _apiClient.GetAsync<GroupDataObject>(
                $"v1.0/myorg/groups/{groupId}",
                cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error retrieving detailed information for group {GroupId}", groupId);
            return null;
        }
    }
}

/// <summary>
/// Response wrapper for groups list
/// </summary>
public class GroupsResponse
{
    [JsonPropertyName("value")]
    public List<GroupDataObject> Value { get; init; } = new();
}