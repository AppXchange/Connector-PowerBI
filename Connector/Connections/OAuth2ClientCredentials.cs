using System;
using Xchange.Connector.SDK.Client.AuthTypes;
using Xchange.Connector.SDK.Client.ConnectionDefinitions.Attributes;

namespace Connector.Connections;

[ConnectionDefinition(
    title: "Microsoft Entra ID OAuth2", 
    description: "OAuth 2.0 client credentials flow for service-to-service authentication with Power BI")]
public class OAuth2ClientCredentials : OAuth2ClientCredentialsBase
{
    [ConnectionProperty(
        title: "Tenant ID",
        description: "The directory tenant ID or domain name (e.g. contoso.onmicrosoft.com)",
        isRequired: true)]
    public string TenantId { get; init; } = string.Empty;

    [ConnectionProperty(
        title: "Token URL",
        description: "The OAuth token endpoint (defaults to https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token)",
        isRequired: true)]
    public new string TokenUrl { get; init; } = "https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";

    [ConnectionProperty(
        title: "Client ID",
        description: "The application (client) ID from Microsoft Entra admin center",
        isRequired: true)]
    public new string ClientId { get; init; } = string.Empty;

    [ConnectionProperty(
        title: "Client Secret",
        description: "The client secret generated in Microsoft Entra admin center",
        isRequired: true,
        isSensitive: true)]
    public new string ClientSecret { get; init; } = string.Empty;

    [ConnectionProperty(
        title: "Scope",
        description: "The Power BI API permission scope (e.g. https://analysis.windows.net/powerbi/api/.default)",
        isRequired: true)]
    public new string Scope { get; init; } = string.Empty;

    [ConnectionProperty(
        title: "Environment",
        description: "The Power BI API environment to connect to",
        isRequired: true)]
    public ConnectionEnvironmentOAuth2ClientCredentials ConnectionEnvironment { get; init; } = 
        ConnectionEnvironmentOAuth2ClientCredentials.Unknown;

    public string BaseUrl => ConnectionEnvironment switch
    {
        ConnectionEnvironmentOAuth2ClientCredentials.Production => "https://api.powerbi.com/v1.0/myorg",
        ConnectionEnvironmentOAuth2ClientCredentials.Test => "https://api.powerbi.com/beta/myorg",
        _ => throw new Exception("No environment was selected.")
    };

    public new ClientAuthentication ClientAuthentication { get; init; } = ClientAuthentication.BasicAuthHeader;

    public string GetFormattedTokenUrl() => TokenUrl.Replace("{tenant}", TenantId);
}

public enum ConnectionEnvironmentOAuth2ClientCredentials
{
    Unknown = 0,
    Production = 1,
    Test = 2
}