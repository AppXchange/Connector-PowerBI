using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;

namespace Connector.Client;

/// <summary>
/// Exception thrown when an API request fails
/// </summary>
public class ApiException : Exception
{
    private int statusCode;


    /// <summary>
    /// HTTP status code of the failed request
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Raw response body from the failed request
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Correlation ID for request tracing
    /// </summary>
    public string? CorrelationId { get; }

    /// <summary>
    /// Parsed error details if available
    /// </summary>
    public ApiErrorDetails? ErrorDetails { get; }

    /// <summary>
    /// Creates a new API exception
    /// </summary>
    public ApiException(string message, HttpStatusCode statusCode, string? responseBody = null, string? correlationId = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        CorrelationId = correlationId;

        if (!string.IsNullOrEmpty(responseBody))
        {
            try
            {
                ErrorDetails = JsonSerializer.Deserialize<ApiErrorDetails>(responseBody);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }
    }

    public ApiException(string? message, int statusCode, string message1) : base(message)
    {
    }

    public ApiException(string? message, int statusCode) : base(message)
    {
        this.statusCode = statusCode;
    }



    /// <summary>
    /// Creates a new API exception from an HTTP response
    /// </summary>
    public static async Task<ApiException> FromResponseAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var correlationId = response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;

        return new ApiException(
            $"API request failed with status code: {response.StatusCode}",
            response.StatusCode,
            body,
            correlationId
        );
    }
}

/// <summary>
/// Structured error details from the API
/// </summary>
public class ApiErrorDetails
{
    /// <summary>
    /// Error code from the API
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Detailed error message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Additional error details
    /// </summary>
    [JsonPropertyName("details")]
    public Dictionary<string, JsonElement>? Details { get; set; }
}