namespace Connector.Groups.v1;
using Connector.Groups.v1.Group;
using Connector.Groups.v1.Group.Create;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Action;

/// <summary>
/// Service definition for the Groups V1 Action Processor module
/// </summary>
public class GroupsV1ActionProcessorServiceDefinition : BaseActionHandlerServiceDefinition<GroupsV1ActionProcessorConfig>
{
    /// <summary>
    /// The unique identifier for this module
    /// </summary>
    public override string ModuleId => "groups-1";

    /// <summary>
    /// The service type that handles actions for this module
    /// </summary>
    public override Type ServiceType => typeof(GenericActionHandlerService<GroupsV1ActionProcessorConfig>);

    /// <summary>
    /// Configures the service dependencies required for this module
    /// </summary>
    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        // Configure JSON serialization options
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        // Deserialize the service configuration
        var serviceConfig = JsonSerializer.Deserialize<GroupsV1ActionProcessorConfig>(serviceConfigJson, options)
            ?? throw new InvalidOperationException("Failed to deserialize service configuration");

        // Register services
        serviceCollection.AddSingleton(serviceConfig);
        serviceCollection.AddSingleton<GenericActionHandlerService<GroupsV1ActionProcessorConfig>>();
        serviceCollection.AddSingleton<IActionHandlerServiceDefinition<GroupsV1ActionProcessorConfig>>(this);

        // Register action handlers
        serviceCollection.AddScoped<CreateGroupHandler>();
    }

    /// <summary>
    /// Configures the action handlers for this module
    /// </summary>
    public override void ConfigureService(IActionHandlerService service, GroupsV1ActionProcessorConfig config)
    {
        // Register the Create Group action handler
        service.RegisterHandlerForDataObjectAction<CreateGroupHandler, GroupDataObject>(
            ModuleId,
            "group", // data object type
            "create", // action type
            config.CreateGroupConfig);
    }
}