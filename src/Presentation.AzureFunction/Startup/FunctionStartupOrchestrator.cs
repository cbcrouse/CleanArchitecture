using AutoMapper;
using Common.Configuration;
using Infrastructure.Startup;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace Presentation.AzureFunction.Startup
{
    /// <summary>
    /// Defines the default behavior for orchestrating dependency registrations across the presentation and infrastructure.
    /// </summary>
    /// <typeparam name="TOrchestrator">The <see cref="CoreStartupOrchestrator"/> implementation.</typeparam>
    public abstract class FunctionStartupOrchestrator<TOrchestrator> : FunctionsStartup where TOrchestrator : CoreStartupOrchestrator, new()
    {
        /// <summary>
        /// Called by the host to configure the application's services.
        /// </summary>
        /// <param name="builder">Defines a class that provides the mechanisms to configure an application's request pipeline.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRoot configuration = SetupConfiguration();
            Configuration = StartupOrchestrator.InitializeConfiguration(configuration);
            ServiceCollection = StartupOrchestrator.InitializeServiceCollection(builder.Services);

            StartupOrchestrator = new TOrchestrator();
            StartupOrchestrator.AddServiceRegistrationExpressions(ServiceRegistrationExpressions);
            StartupOrchestrator.AddMapperExtensionExpressions(MapperExtensionExpressions);
            StartupOrchestrator.Orchestrate();
        }

        /// <summary>
        /// This interface exposes functions to tap into the service registration pipeline from external layers of the Application.
        /// </summary>
        protected TOrchestrator StartupOrchestrator { get; set; } = new TOrchestrator();

        /// <summary>
        /// Specifies the contract for a collection of service descriptors.
        /// </summary>
        protected IServiceCollection ServiceCollection { get; set; } = new ServiceCollection();

        /// <summary>
        /// Represents a set of key/value application configuration properties.
        /// </summary>
        protected IConfiguration Configuration { get; set; } = new ConfigurationBuilder().Build();

        /// <summary>
        /// Sets up the <see cref="IConfigurationRoot"/> from a new <see cref="ConfigurationBuilder"/>.
        /// </summary>
        protected virtual IConfigurationRoot SetupConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            SetBasePath(builder);
            AddConfigurations(builder);
            ExtendConfigurationBuilder(builder);
            return builder.Build();
        }

        /// <summary>
        /// Sets the default base path for configuration files.
        /// </summary>
        /// <param name="builder">Represents a type used to build application configuration.</param>
        protected virtual void SetBasePath(IConfigurationBuilder builder)
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = Directory.GetParent(assemblyLocation).FullName;
            builder.SetBasePath(path);
        }

        /// <summary>
        /// Adds the default configurations on the <see cref="IConfigurationBuilder"/>.
        /// </summary>
        /// <param name="builder">Represents a type used to build application configuration.</param>
        protected virtual void AddConfigurations(IConfigurationBuilder builder)
        {
            string environment = EnvironmentHelper.GetRequiredEnvironmentVariable("ASPNETCORE_Environment");

            // Load application settings first
            builder.AddJsonFile("appsettings.core.json", optional: false, reloadOnChange: true);
            if (!string.IsNullOrWhiteSpace(environment))
                builder.AddJsonFile($"appsettings.core.{environment}.json", optional: true, reloadOnChange: true);

            // Load presentation settings next
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            if (!string.IsNullOrWhiteSpace(environment))
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            // Load environment variables next to last
            builder.AddEnvironmentVariables();

            // Load local developer settings last
            builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
        }

        /// <summary>
        /// Change the current state of the <see cref="IConfigurationBuilder"/> after default settings have been configured.
        /// </summary>
        /// <remarks>
        /// This method should never contain an implementation. It is an optional way
        /// for the implementer should override without forcing the implementation.
        /// </remarks>
        /// <param name="builder">Represents a type used to build application configuration.</param>
        protected virtual void ExtendConfigurationBuilder(IConfigurationBuilder builder) { }

        /// <summary>
        /// A list of expressions that will be invoked by the <see cref="CoreStartupOrchestrator"/>.
        /// Expressions in this list should register all required services of the application as well as the presentation to the <see cref="IServiceCollection"/>.
        /// </summary>
        protected IList<Expression<Action>> ServiceRegistrationExpressions { get; set; } = new List<Expression<Action>>();

        /// <summary>
        /// A list of expressions that will be invoked by the <see cref="CoreStartupOrchestrator"/>.
        /// Expressions in this list should register profiles for AutoMapper and perform other AutoMapper configurations.
        /// Access to <see cref="IMapperConfigurationExpression"/> is available in this expression list.
        /// </summary>
        protected IList<Expression<Action<IMapperConfigurationExpression>>> MapperExtensionExpressions { get; set; } = new List<Expression<Action<IMapperConfigurationExpression>>>();
    }
}
