using System;
using AutoMapper;
using Common.Configuration;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mapping.Tests
{
	public partial class MappingTests
	{
		public IMapper Sut { get; }

		public MappingTests()
		{
			var serviceCollection = new ServiceCollection();
			var configurationBuilder = new ConfigurationBuilder();
			Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
			configurationBuilder.AddPrioritizedSettings();
			var configuration = configurationBuilder.Build();
			var orchestrator = new AppStartupOrchestrator();
			orchestrator.InitializeConfiguration(configuration);
			orchestrator.InitializeServiceCollection(serviceCollection);
			orchestrator.Orchestrate();

			var serviceProvider = serviceCollection.BuildServiceProvider();
			Sut = serviceProvider.GetRequiredService<IMapper>();
		}
	}
}
