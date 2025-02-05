namespace Connector.PushDatasets.v1.Table;

using Json.Schema.Generation;
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
[PrimaryKey("name", nameof(Name))]
[Description("Represents a table within a Power BI push dataset")]
public class TableDataObject
{
    [JsonPropertyName("name")]
    [Description("The table name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("columns")]
    [Description("The column schema for this table")]
    public List<Column> Columns { get; init; } = new();

    [JsonPropertyName("measures")]
    [Description("The measures within this table")]
    public List<Measure>? Measures { get; init; }

    [JsonPropertyName("description")]
    [Description("The table description")]
    public string? Description { get; init; }

    [JsonPropertyName("isHidden")]
    [Description("Whether this table is hidden")]
    public bool? IsHidden { get; init; }
}

public class Column
{
    [JsonPropertyName("name")]
    [Description("The name of the column")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("dataType")]
    [Description("The data type of the column")]
    public string DataType { get; init; } = string.Empty;

    [JsonPropertyName("formatString")]
    [Description("The format string for the column")]
    public string? FormatString { get; init; }

    [JsonPropertyName("sortByColumn")]
    [Description("The column used for sorting")]
    public string? SortByColumn { get; init; }

    [JsonPropertyName("dataCategory")]
    [Description("The data category of the column")]
    public string? DataCategory { get; init; }

    [JsonPropertyName("isHidden")]
    [Description("Whether this column is hidden")]
    public bool? IsHidden { get; init; }
}

public class Measure
{
    [JsonPropertyName("name")]
    [Description("The name of the measure")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("expression")]
    [Description("The DAX expression for the measure")]
    public string Expression { get; init; } = string.Empty;

    [JsonPropertyName("formatString")]
    [Description("The format string for the measure")]
    public string? FormatString { get; init; }

    [JsonPropertyName("isHidden")]
    [Description("Whether this measure is hidden")]
    public bool? IsHidden { get; init; }
}