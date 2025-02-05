using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Collections.Generic;

namespace Connector.Client;

public class TokenProvider : ITokenProvider
{
    private readonly ConnectorRegistrationConfig _config;
    private readonly ILogger<TokenProvider> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public TokenProvider(ConnectorRegistrationConfig config, ILogger<TokenProvider> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetToken(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _config.Auth.ClientId,
                ["client_secret"] = _config.Auth.ClientSecret,
                ["scope"] = _config.Auth.Scope
            });

            var response = await client.PostAsync(_config.Auth.TokenUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            if (result == null)
            {
                throw new ApiException("Failed to deserialize token response", (int)response.StatusCode);
            }

            _cachedToken = result.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 300); // Refresh 5 minutes before expiry
            return _cachedToken;
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            _logger.LogError(ex, "Failed to acquire token");
            throw new ApiException("Token acquisition failed", 500, ex.Message);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task InvalidateToken()
    {
        _cachedToken = null;
        _tokenExpiry = DateTime.MinValue;
        return Task.CompletedTask;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";
    }
} 