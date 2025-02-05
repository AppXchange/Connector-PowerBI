namespace Connector.Datasets.v1.Dataset;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Represents a Power BI dataset. Contains properties returned by the Power BI REST API.
/// </summary>
[PrimaryKey("id", nameof(Id))]
[Description("A Power BI dataset containing properties like name, owner, refresh settings, etc.")]
public class DatasetDataObject
{
    [JsonPropertyName("id")]
    [Description("The dataset ID")]
    public required string Id { get; init; }

    [JsonPropertyName("name")] 
    [Description("The dataset name")]
    public required string Name { get; init; }

    [JsonPropertyName("addRowsAPIEnabled")]
    [Description("Whether the dataset allows adding new rows")]
    public bool AddRowsAPIEnabled { get; init; }

    [JsonPropertyName("configuredBy")]
    [Description("The dataset owner")]
    public string? ConfiguredBy { get; init; }

    [JsonPropertyName("isRefreshable")]
    [Description("Whether the dataset can be refreshed. Returns true when the dataset is either recently refreshed or configured for automatic refresh with Import connection mode")]
    public bool IsRefreshable { get; init; }

    [JsonPropertyName("isEffectiveIdentityRequired")]
    [Description("Whether the dataset requires an effective identity for GenerateToken API calls")]
    public bool IsEffectiveIdentityRequired { get; init; }

    [JsonPropertyName("isEffectiveIdentityRolesRequired")]
    [Description("Whether row-level security is defined requiring a role to be specified")]
    public bool IsEffectiveIdentityRolesRequired { get; init; }

    [JsonPropertyName("isOnPremGatewayRequired")]
    [Description("Whether the dataset requires an on-premises data gateway")]
    public bool IsOnPremGatewayRequired { get; init; }

    [JsonPropertyName("targetStorageMode")]
    [Description("The dataset storage mode")]
    public string? TargetStorageMode { get; init; }

    [JsonPropertyName("createReportEmbedURL")]
    [Description("The dataset create report embed URL")]
    public string? CreateReportEmbedURL { get; init; }

    [JsonPropertyName("qnaEmbedURL")]
    [Description("The dataset Q&A embed URL")] 
    public string? QnaEmbedURL { get; init; }

    [JsonPropertyName("description")]
    [Description("The dataset description")]
    public string? Description { get; init; }

    [JsonPropertyName("webUrl")]
    [Description("The web URL of the dataset")]
    public required string WebUrl { get; init; }

    [JsonPropertyName("isInPlaceSharingEnabled")]
    [Description("Whether the dataset can be shared with external users to be consumed in their own tenant")]
    public bool IsInPlaceSharingEnabled { get; init; }
}