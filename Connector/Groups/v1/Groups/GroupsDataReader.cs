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
using Connector.Groups.v1.Groups;

namespace Connector.Groups.v1.Groups;

public class GroupsDataReader : TypedAsyncDataReaderBase<GroupsDataObject>
{
    private readonly ApiClient _apiClient;

    public GroupsDataReader(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<GroupsDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetGroups(cancellationToken);
        if (response.Data?.Value != null)
        {
            foreach (var group in response.Data.Value)
            {
                yield return group;
            }
        }
    }
}