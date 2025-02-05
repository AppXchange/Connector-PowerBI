using System.Text.Json.Serialization;

namespace Connector;

/// <summary>
/// Contains configuration values necessary for the Power BI connector
/// </summary>
public class ConnectorRegistrationConfig
{
    /// <summary>
    /// Whether to enable real-time action processing
    /// </summary>
    [JsonPropertyName("realTimeActionProcessing")]
    public bool RealTimeActionProcessing { get; set; } = true;

    /// <summary>
    /// Base URL for the Power BI API
    /// </summary>
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = "https://api.powerbi.com/v1.0/myorg";

    /// <summary>
    /// Authentication configuration
    /// </summary>
    [JsonPropertyName("auth")]
    public AuthConfig Auth { get; set; } = new AuthConfig();

    /// <summary>
    /// Default retention policy for push datasets
    /// </summary>
    [JsonPropertyName("defaultRetentionPolicy")] 
    public string DefaultRetentionPolicy { get; set; } = "basicFIFO";
}

public class AuthConfig
{
    /// <summary>
    /// OAuth token endpoint URL
    /// </summary>
    [JsonPropertyName("tokenUrl")]
    public string TokenUrl { get; set; } = "https://login.microsoftonline.com/common/oauth2/token";

    /// <summary>
    /// Client ID for OAuth authentication
    /// </summary>
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for OAuth authentication
    /// </summary>
    [JsonPropertyName("clientSecret")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// OAuth scope
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = "Dataset.ReadWrite.All";
}
