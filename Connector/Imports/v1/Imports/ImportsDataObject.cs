namespace Connector.Imports.v1.Imports;

using Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Data object that will represent an object in the Xchange system. This will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// These types will be used for validation at runtime to make sure the objects being passed through the system 
/// are properly formed. The schema also helps provide integrators more information for what the values 
/// are intended to be.
/// </summary>
[PrimaryKey("id", nameof(Id))]
//[AlternateKey("alt-key-id", nameof(CompanyId), nameof(EquipmentNumber))]
[Description("Represents a Power BI import object in a collection")]
public class ImportsDataObject
{
    [JsonPropertyName("id")]
    [Description("The import ID")]
    public required string Id { get; init; }

    [JsonPropertyName("importState")]
    [Description("The import upload state")]
    public required string ImportState { get; init; }

    [JsonPropertyName("createdDateTime")]
    [Description("Import creation date and time")]
    public required string CreatedDateTime { get; init; }

    [JsonPropertyName("updatedDateTime")]
    [Description("Import last update date and time")]
    public required string UpdatedDateTime { get; init; }

    [JsonPropertyName("name")]
    [Description("The import name")]
    public required string Name { get; init; }

    [JsonPropertyName("connectionType")]
    [Description("The import connection type")]
    public required string ConnectionType { get; init; }

    [JsonPropertyName("source")]
    [Description("The source of the import")]
    public required string Source { get; init; }

    [JsonPropertyName("datasets")]
    [Description("The datasets associated with this import")]
    public required List<Dataset> Datasets { get; init; }

    [JsonPropertyName("reports")]
    [Description("The reports associated with this import")]
    public required List<Report> Reports { get; init; }

    public class Dataset
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("webUrl")]
        public required string WebUrl { get; init; }
    }

    public class Report
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("webUrl")]
        public required string WebUrl { get; init; }

        [JsonPropertyName("embedUrl")]
        public required string EmbedUrl { get; init; }
    }
}