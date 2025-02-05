namespace Connector.Imports.v1.Imports;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ImportsResponse
{
    [JsonPropertyName("value")]
    public required List<ImportsDataObject> Value { get; init; }
} 