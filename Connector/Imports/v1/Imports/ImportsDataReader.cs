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
using System.Text.Json;
using System.Threading.Tasks;

namespace Connector.Imports.v1.Imports;

public class ImportsDataReader : TypedAsyncDataReaderBase<ImportsDataObject>
{
    private readonly ApiClient _apiClient;

    public ImportsDataReader(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async IAsyncEnumerable<ImportsDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var response = await _apiClient.GetImports(cancellationToken);
        if (response.Data?.Value != null)
        {
            foreach (var import in response.Data.Value)
            {
                yield return import;
            }
        }
    }
}