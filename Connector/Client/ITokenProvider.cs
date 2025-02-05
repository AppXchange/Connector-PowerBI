using System.Threading;
using System.Threading.Tasks;

namespace Connector.Client;

public interface ITokenProvider
{
    /// <summary>
    /// Gets a valid OAuth token for API authentication
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Valid OAuth token</returns>
    Task<string> GetToken(CancellationToken cancellationToken);

    /// <summary>
    /// Forces token refresh regardless of cache
    /// </summary>
    Task InvalidateToken();
} 