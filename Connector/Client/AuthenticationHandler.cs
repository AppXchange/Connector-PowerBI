using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Connector.Client;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthenticationHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetToken(cancellationToken);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
} 