using Connector.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Client.Testing;
using System.Threading;

namespace Connector.Connections
{
    public class ConnectionTestHandler : IConnectionTestHandler
    {
        private readonly ILogger<IConnectionTestHandler> _logger;
        private readonly ApiClient _apiClient;

        public ConnectionTestHandler(ILogger<IConnectionTestHandler> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public Task<TestConnectionResult> TestConnection()
        {
            return TestConnection(CancellationToken.None);
        }

        public async Task<TestConnectionResult> TestConnection(CancellationToken cancellationToken)
        {
            try
            {
                var success = await _apiClient.TestConnectionAsync(cancellationToken);
                return new TestConnectionResult
                {
                    Success = success,
                    Message = success ? "Successfully connected to Power BI API" : "Failed to connect to Power BI API",
                    StatusCode = success ? 200 : 500
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection");
                return new TestConnectionResult
                {
                    Success = false,
                    Message = $"Connection test failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
