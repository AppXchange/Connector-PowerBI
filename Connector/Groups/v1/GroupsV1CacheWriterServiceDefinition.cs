namespace Connector.Groups.v1;
using Connector.Groups.v1.Group;
using Connector.Groups.v1.Groups;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Abstraction.Change;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Hosting.Configuration;

/// <summary>
/// Service definition for the Groups V1 Cache Writer module
/// </summary>
public class GroupsV1CacheWriterServiceDefinition : BaseCacheWriterServiceDefinition<GroupsV1CacheWriterConfig>
{
    /// <summary>
    /// The unique identifier for this module
    /// </summary>
    public override string ModuleId => "groups-1";

    /// <summary>
    /// The service type that handles cache writing for this module
    /// </summary>
    public override Type ServiceType => typeof(GenericCacheWriterService<GroupsV1CacheWriterConfig>);

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
        var serviceConfig = JsonSerializer.Deserialize<GroupsV1CacheWriterConfig>(serviceConfigJson, options)
            ?? throw new InvalidOperationException("Failed to deserialize service configuration");

        // Register services
        serviceCollection.AddSingleton(serviceConfig);
        serviceCollection.AddSingleton<GenericCacheWriterService<GroupsV1CacheWriterConfig>>();
        serviceCollection.AddSingleton<ICacheWriterServiceDefinition<GroupsV1CacheWriterConfig>>(this);

        // Register data readers
        serviceCollection.AddSingleton<GroupDataReader>();
        serviceCollection.AddSingleton<GroupsDataReader>();
    }

    /// <summary>
    /// Configures the change detection system for this module
    /// </summary>
    public override IDataObjectChangeDetectorProvider ConfigureChangeDetectorProvider(
        IChangeDetectorFactory factory, 
        ConnectorDefinition connectorDefinition)
    {
        var options = factory.CreateProviderOptionsWithNoDefaultResolver();

        // Configure data object keys for change detection
        this.RegisterKeysForObject<GroupDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<GroupsDataObject>(options, connectorDefinition);

        return factory.CreateProvider(options);
    }

    /// <summary>
    /// Configures the cache writer service for this module
    /// </summary>
    public override void ConfigureService(ICacheWriterService service, GroupsV1CacheWriterConfig config)
    {
        // Configure standard data reader settings
        var dataReaderSettings = new DataReaderSettings
        {
            DisableDeletes = false, // Enable delete detection
            UseChangeDetection = true // Enable change detection system
        };

        // Register data readers with their configurations
        service.RegisterDataReader<GroupDataReader, GroupDataObject>(
            ModuleId,
            config.GroupConfig,
            dataReaderSettings);

        service.RegisterDataReader<GroupsDataReader, GroupsDataObject>(
            ModuleId,
            config.GroupsConfig,
            dataReaderSettings);
    }
}