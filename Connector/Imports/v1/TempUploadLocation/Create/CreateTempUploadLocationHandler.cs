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

namespace Connector.Imports.v1.TempUploadLocation.Create;

public class CreateTempUploadLocationHandler : IActionHandler<CreateTempUploadLocationAction>
{
    private readonly ILogger<CreateTempUploadLocationHandler> _logger;
    private readonly IApiClient _apiClient;

    public CreateTempUploadLocationHandler(
        ILogger<CreateTempUploadLocationHandler> logger,
        IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance, 
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<CreateTempUploadLocationActionInput>(actionInstance.InputJson)
            ?? throw new ArgumentException("Invalid input JSON");

        try
        {
            // Define the endpoint based on whether we have a group ID
            var endpoint = !string.IsNullOrEmpty(input.GroupId)
                ? $"v1.0/myorg/groups/{input.GroupId}/imports/createTemporaryUploadLocation"
                : "v1.0/myorg/imports/createTemporaryUploadLocation";

            ApiResponse<CreateTempUploadLocationActionOutput> response = await _apiClient.PostAsync<ApiResponse<CreateTempUploadLocationActionOutput>>(
                endpoint,
                new StringContent(string.Empty), // Empty content instead of null
                cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to create temporary upload location. API StatusCode: {response.StatusCode}");
            }

            if (response.Data == null)
            {
                throw new Exception("Response data was null");
            }

            // Build sync operations
            var operations = new List<SyncOperation>();
            var keyResolver = new DefaultDataObjectKey();
            var key = keyResolver.BuildKeyResolver()(response.Data);
            operations.Add(SyncOperation.CreateSyncOperation(
                UpdateOperation.Upsert.ToString(), 
                key.UrlPart, 
                key.PropertyNames, 
                response.Data));

            var resultList = new List<CacheSyncCollection>
            {
                new() { 
                    DataObjectType = typeof(TempUploadLocationDataObject), 
                    CacheChanges = operations.ToArray() 
                }
            };

            return ActionHandlerOutcome.Successful(response.Data, resultList);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while creating temporary upload location");
            
            var errorSource = new List<string> { "CreateTempUploadLocationHandler" };
            if (!string.IsNullOrEmpty(ex.Source)) 
                errorSource.Add(ex.Source);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = ex.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = errorSource.ToArray(),
                        Text = ex.Message
                    }
                }
            });
        }
    }

    public async Task<ApiResponse<CreateTempUploadLocationActionOutput>> HandleAsync(
        CreateTempUploadLocationActionInput input, 
        CancellationToken cancellationToken)
    {
        var response = await _apiClient.CreateImport<CreateTempUploadLocationActionOutput>(input, cancellationToken);
        return response;
    }
}
