namespace Connector.Groups.v1.Groups;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GroupsResponse
{
    [JsonPropertyName("value")]
    public required List<GroupsDataObject> Value { get; set; }
} 