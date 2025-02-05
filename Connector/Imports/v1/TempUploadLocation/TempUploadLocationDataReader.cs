namespace Connector.Imports.v1.TempUploadLocation;

using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.CacheWriter;

public class TempUploadLocationDataReader : TypedAsyncDataReaderBase<TempUploadLocationDataObject>
{
    private readonly ILogger<TempUploadLocationDataReader> _logger;

    public TempUploadLocationDataReader(ILogger<TempUploadLocationDataReader> logger)
    {
        _logger = logger;
    }

    public override async IAsyncEnumerable<TempUploadLocationDataObject> GetTypedDataAsync(
        DataObjectCacheWriteArguments? dataObjectRunArguments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Satisfy async requirement
        // Temporary upload locations are created on-demand and not listed via API
        yield break;
    }
} 