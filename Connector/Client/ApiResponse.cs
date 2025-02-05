using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Connector.Groups.v1.Group;
using Connector.Imports.v1.Import;
using Connector.Imports.v1.Import.Create;
using Connector.Imports.v1.Imports;
using Connector.Imports.v1.TempUploadLocation.Create;

namespace Connector.Client;

/// <summary>
/// Represents a standardized API response
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the API request was successful
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// HTTP status code of the response
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Typed response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Raw response content for debugging
    /// </summary>
    public Stream? RawResult { get; set; }

    /// <summary>
    /// Error message if the request failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Correlation ID for request tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Request timestamp
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            IsSuccessful = true,
            StatusCode = HttpStatusCode.OK,
            Data = data
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message, Exception? exception = null)
    {
        return new ApiResponse<T>
        {
            IsSuccessful = false,
            StatusCode = statusCode,
            ErrorMessage = message,
            Data = default
        };
    }

    /// <summary>
    /// Attempts to deserialize the raw result into the Data property
    /// </summary>
    public bool TryDeserializeRawResult(JsonSerializerOptions? options = null)
    {
        if (RawResult == null || !RawResult.CanRead)
            return false;

        try
        {
            RawResult.Position = 0;
            Data = JsonSerializer.Deserialize<T>(RawResult, options);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Non-generic API response for responses without data
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse Success()
    {
        return new ApiResponse
        {
            IsSuccessful = true,
            StatusCode = HttpStatusCode.OK
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public new static ApiResponse Error(HttpStatusCode statusCode, string message, Exception? exception = null)
    {
        return new ApiResponse
        {
            IsSuccessful = false,
            StatusCode = statusCode,
            ErrorMessage = message
        };
    }
}