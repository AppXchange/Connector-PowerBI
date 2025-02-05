namespace Connector;
using Connector.Client;
using Connector.Connections;
using Connector.Datasets.v1;
using Connector.Groups.v1;
using Connector.Imports.v1;
using Connector.PushDatasets.v1;
using ESR.Hosting;
using ESR.Hosting.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using Xchange.Connector.SDK.Abstraction.Hosting;
using Xchange.Connector.SDK.Client.Testing;

/// <summary>
/// The registration object for your connector. This will be passed to the <see cref = "Extensions.UseGenericServiceRun"/> method at
/// Program startup. <see cref = "Program.Main(string[])"/>
/// </summary>
public class ConnectorRegistration : IConnectorRegistration<ConnectorRegistrationConfig>, IConfigureConnectorApiClient
{
    /// <summary>
    /// Registers any objects that are needed for dependency injection for the connector. 
    /// </summary>
    /// <param name = "serviceCollection"><see cref = "IServiceCollection"/> to register connector types with.</param>
    /// <param name = "hostContext">Host context that provides any configuration for the service run.</param>
    public void ConfigureServices(IServiceCollection serviceCollection, IHostContext hostContext)
    {
        // Parse and register configuration
        var config = JsonSerializer.Deserialize<ConnectorRegistrationConfig>(
            hostContext.GetSystemConfig().Configuration);
        
        if (config == null)
        {
            throw new InvalidOperationException("Failed to deserialize connector configuration");
        }

        serviceCollection.AddSingleton(config);

        // Add HTTP client handlers
        serviceCollection.AddTransient<RetryPolicyHandler>();
        serviceCollection.AddTransient<AuthenticationHandler>();

        // Add core services
        serviceCollection.AddHttpClient("PowerBIClient", client =>
        {
            client.BaseAddress = new Uri(config.BaseUrl);
        })
        .AddHttpMessageHandler<AuthenticationHandler>()
        .AddHttpMessageHandler<RetryPolicyHandler>();

        // Add other required services
        serviceCollection.AddSingleton<IPowerBIClient, PowerBIClient>();
        serviceCollection.AddSingleton<ITokenProvider, TokenProvider>();
    }

    /// <summary>
    /// Registers all <see cref = "IConnectorServiceDefinition"/> implementations. If using the xchange CLI tooling, it will normally
    /// add these for you when adding a new module to the connector. Most modules will have an Action processor service definition
    /// and a Cache Writer service definition
    /// </summary>
    /// <param name = "serviceCollection"></param>
    public void RegisterServiceDefinitions(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConnectorServiceDefinition, GroupsV1ActionProcessorServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, GroupsV1CacheWriterServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, DatasetsV1ActionProcessorServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, DatasetsV1CacheWriterServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, ImportsV1ActionProcessorServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, ImportsV1CacheWriterServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, PushDatasetsV1ActionProcessorServiceDefinition>();
        serviceCollection.AddSingleton<IConnectorServiceDefinition, PushDatasetsV1CacheWriterServiceDefinition>();
    }

    public void ConfigureConnectorApiClient(IServiceCollection serviceCollection, IHostConnectionContext hostConnectionContext)
    {
        var activeConnection = hostConnectionContext.GetConnection();
        serviceCollection.ResolveServices(activeConnection);
    }

    public void RegisterConnectionTestHandler(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConnectionTestHandler, ConnectionTestHandler>();
    }
}