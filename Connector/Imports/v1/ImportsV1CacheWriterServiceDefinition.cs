namespace Connector.Imports.v1;
using Connector.Imports.v1.Import;
using Connector.Imports.v1.Imports;
using Connector.Imports.v1.TempUploadLocation;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using Xchange.Connector.SDK.Abstraction.Change;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Hosting.Configuration;

public class ImportsV1CacheWriterServiceDefinition : BaseCacheWriterServiceDefinition<ImportsV1CacheWriterConfig>
{
    public override string ModuleId => "imports-1";
    public override Type ServiceType => typeof(GenericCacheWriterService<ImportsV1CacheWriterConfig>);

    public override void ConfigureServiceDependencies(IServiceCollection serviceCollection, string serviceConfigJson)
    {
        var serviceConfig = JsonSerializer.Deserialize<ImportsV1CacheWriterConfig>(serviceConfigJson);
        serviceCollection.AddSingleton<ImportsV1CacheWriterConfig>(serviceConfig!);
        serviceCollection.AddSingleton<GenericCacheWriterService<ImportsV1CacheWriterConfig>>();
        serviceCollection.AddSingleton<ICacheWriterServiceDefinition<ImportsV1CacheWriterConfig>>(this);
        // Register Data Readers as Singletons
        serviceCollection.AddSingleton<ImportDataReader>();
        serviceCollection.AddSingleton<ImportsDataReader>();
        serviceCollection.AddSingleton<TempUploadLocationDataReader>();
    }

    public override IDataObjectChangeDetectorProvider ConfigureChangeDetectorProvider(IChangeDetectorFactory factory, ConnectorDefinition connectorDefinition)
    {
        var options = factory.CreateProviderOptionsWithNoDefaultResolver();
        // Configure Data Object Keys for Data Objects that do not use the default
        this.RegisterKeysForObject<ImportDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<ImportsDataObject>(options, connectorDefinition);
        this.RegisterKeysForObject<TempUploadLocationDataObject>(options, connectorDefinition);
        return factory.CreateProvider(options);
    }

    public override void ConfigureService(ICacheWriterService service, ImportsV1CacheWriterConfig config)
    {
        var dataReaderSettings = new DataReaderSettings
        {
            DisableDeletes = false,
            UseChangeDetection = true
        };
        // Register Data Reader configurations for the Cache Writer Service
        service.RegisterDataReader<ImportDataReader, ImportDataObject>(ModuleId, config.ImportConfig, dataReaderSettings);
        service.RegisterDataReader<ImportsDataReader, ImportsDataObject>(ModuleId, config.ImportsConfig, dataReaderSettings);
        service.RegisterDataReader<TempUploadLocationDataReader, TempUploadLocationDataObject>(ModuleId, config.TempUploadLocationConfig, dataReaderSettings);
    }
}