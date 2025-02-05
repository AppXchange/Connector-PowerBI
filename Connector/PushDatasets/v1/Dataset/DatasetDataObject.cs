namespace Connector.PushDatasets.v1.Dataset;

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
[Description("Represents a Power BI dataset that allows programmatic access for pushing data.")]
public class DatasetDataObject
{
    [JsonPropertyName("id")]
    [Description("The unique identifier of the dataset")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    [Description("The name of the dataset")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("defaultMode")]
    [Description("The dataset mode or type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatasetMode DefaultMode { get; init; } = DatasetMode.Push;

    [JsonPropertyName("tables")]
    [Description("The tables within the dataset")]
    public List<Table> Tables { get; init; } = new();

    [JsonPropertyName("relationships")]
    [Description("The relationships between tables in the dataset")]
    public List<Relationship>? Relationships { get; init; }

    [JsonPropertyName("defaultRetentionPolicy")]
    [Description("The default retention policy for the dataset")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DefaultRetentionPolicy? DefaultRetentionPolicy { get; init; }

    [JsonPropertyName("isRefreshable")]
    [Description("Indicates if the dataset can be refreshed")]
    public bool IsRefreshable { get; init; }

    [JsonPropertyName("isEffectiveIdentityRequired")]
    [Description("Whether the dataset requires an effective identity")]
    public bool IsEffectiveIdentityRequired { get; init; }

    [JsonPropertyName("isEffectiveIdentityRolesRequired")]
    [Description("Whether row-level security is defined")]
    public bool IsEffectiveIdentityRolesRequired { get; init; }

    [JsonPropertyName("isOnPremGatewayRequired")]
    [Description("Whether the dataset requires an on-premises data gateway")]
    public bool IsOnPremGatewayRequired { get; init; }
}

public class Table
{
    [JsonPropertyName("name")]
    [Description("The name of the table")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("columns")]
    [Description("The columns within the table")]
    public List<Column> Columns { get; init; } = new();

    [JsonPropertyName("measures")]
    [Description("The measures within the table")]
    public List<Measure>? Measures { get; init; }

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

public class Relationship
{
    [JsonPropertyName("name")]
    [Description("The name of the relationship")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("fromTable")]
    [Description("The foreign key table")]
    public string FromTable { get; init; } = string.Empty;

    [JsonPropertyName("fromColumn")]
    [Description("The foreign key column")]
    public string FromColumn { get; init; } = string.Empty;

    [JsonPropertyName("toTable")] 
    [Description("The primary key table")]
    public string ToTable { get; init; } = string.Empty;

    [JsonPropertyName("toColumn")]
    [Description("The primary key column")]
    public string ToColumn { get; init; } = string.Empty;

    [JsonPropertyName("crossFilteringBehavior")]
    [Description("The filter direction of the relationship")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CrossFilteringBehavior CrossFilteringBehavior { get; init; } = CrossFilteringBehavior.OneDirection;
}

public enum DatasetMode
{
    Push,
    PushStreaming,
    Streaming,
    AsAzure,
    AsOnPrem
}

public enum DefaultRetentionPolicy
{
    None,
    BasicFIFO
}

public enum CrossFilteringBehavior
{
    OneDirection,
    BothDirections,
    Automatic
}