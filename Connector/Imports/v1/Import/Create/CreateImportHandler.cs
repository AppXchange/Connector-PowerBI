using Connector.Client;
using ESR.Hosting.Action;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Action;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Client.AppNetwork;

namespace Connector.Imports.v1.Import.Create;

public class CreateImportHandler : IActionHandler<CreateImportAction>
{
    private readonly ILogger<CreateImportHandler> _logger;
    private readonly IApiClient _apiClient;

    public CreateImportHandler(
        ILogger<CreateImportHandler> logger,
        IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }
    
    public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(
        ActionInstance actionInstance, 
        CancellationToken cancellationToken)
    {
        var input = JsonSerializer.Deserialize<CreateImportActionInput>(actionInstance.InputJson)
            ?? throw new ArgumentException("Invalid input JSON");

        try
        {
            // Build the URL with query parameters
            var queryParams = new List<string>
            {
                $"datasetDisplayName={Uri.EscapeDataString(input.DatasetDisplayName)}"
            };

            if (input.NameConflict.HasValue)
                queryParams.Add($"nameConflict={input.NameConflict}");
            if (input.SkipReport.HasValue)
                queryParams.Add($"skipReport={input.SkipReport}");
            if (input.OverrideReportLabel.HasValue)
                queryParams.Add($"overrideReportLabel={input.OverrideReportLabel}");
            if (input.OverrideModelLabel.HasValue)
                queryParams.Add($"overrideModelLabel={input.OverrideModelLabel}");

            var url = !string.IsNullOrEmpty(input.GroupId)
                ? $"v1.0/myorg/groups/{input.GroupId}/imports?{string.Join("&", queryParams)}"
                : $"v1.0/myorg/imports?{string.Join("&", queryParams)}";

            // Create multipart form content
            var boundary = $"--{Guid.NewGuid()}";
            var content = new MultipartFormDataContent(boundary);
            
            var fileBytes = Convert.FromBase64String(input.FileContent);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "file", input.DatasetDisplayName);

            // Make the API call
            var response = await _apiClient.PostAsync<ApiResponse<CreateImportActionOutput>>(
                url,
                content,
                cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to create import. API StatusCode: {response.StatusCode}");
            }

            if (response.Data == null)
            {
                throw new Exception("Import response data was null");
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
                    DataObjectType = typeof(ImportDataObject), 
                    CacheChanges = operations.ToArray() 
                }
            };

            return ActionHandlerOutcome.Successful(response.Data, resultList);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Error creating import");
            
            var errorSource = new List<string> { "CreateImportHandler" };
            if (!string.IsNullOrEmpty(exception.Source)) 
                errorSource.Add(exception.Source);
            
            return ActionHandlerOutcome.Failed(new StandardActionFailure
            {
                Code = exception.StatusCode?.ToString() ?? "500",
                Errors = new[]
                {
                    new Xchange.Connector.SDK.Action.Error
                    {
                        Source = errorSource.ToArray(),
                        Text = exception.Message
                    }
                }
            });
        }
    }

    public async Task<ApiResponse<CreateImportActionOutput>> HandleAsync(
        CreateImportActionInput input, 
        CancellationToken cancellationToken)
    {
        var response = await _apiClient.CreateImport<CreateImportActionOutput>(input, cancellationToken);
        return response;
    }
}
