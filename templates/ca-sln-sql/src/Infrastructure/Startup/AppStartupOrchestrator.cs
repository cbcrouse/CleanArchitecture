using System;
using Application.Configuration;
using Application.Interfaces;
using Application.Interfaces.ValueResolvers;
using Application.Mapping;
using AutoMapper;
using AutoMapper.Configuration;
using FluentValidation;
using Infrastructure.Configuration;
using Infrastructure.MediatR;
using Infrastructure.Services;
using Infrastructure.ValueResolvers;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Persistence.Sql;
using Persistence.Sql.Configuration;
using Persistence.Sql.TodoItem;

namespace Infrastructure.Startup
{
	/// <summary>
	/// Facilitates dependency registrations for the application.
	/// </summary>
	public class AppStartupOrchestrator : CoreStartupOrchestrator
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public AppStartupOrchestrator()
		{
			// Configuration Options
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddOptions());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsFactory<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsMonitor<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<ApplicationOptions>(Configuration));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<InfrastructureOptions>(Configuration));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<PersistenceOptions>(Configuration));

			// Add MediatR and FluentValidation
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddMediatR(GetMediatRConfiguration()));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddValidatorsFromAssemblyContaining(typeof(ApplicationOptions), ServiceLifetime.Transient, null, false));

			// AutoMapper
			ServiceRegistrationExpressions.Add(() => RegisterAutoMapper());

			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<ISystemClock, SystemClock>());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<IKeyGenerator, MongoKeyGenerator>());

			ServiceRegistrationExpressions.Add(() => AddSqlDataAccess());
		}

        private static Action<MediatRServiceConfiguration> GetMediatRConfiguration()
        {
            return configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(ApplicationOptions).Assembly);
            };
        }

        private void RegisterAutoMapper()
		{
			ServiceCollection.AddSingleton(serviceProvider =>
			{
				var configurationExpression = new MapperConfigurationExpression();
				AugmentExpressionExecution(() => configurationExpression.ConstructServicesUsing(serviceProvider.GetService));
				AugmentExpressionExecutions(MapperExtensionExpressions, configurationExpression);
				var configuration = new MapperConfiguration(configurationExpression);
				return configuration.CreateMapper();
			});

			// Profiles
			MapperExtensionExpressions.Add(mapperConfig => mapperConfig.AddProfile(typeof(ApplicationProfile)));

			// AutoMapper Resolvers
			ServiceCollection.AddSingleton(typeof(INowValueResolver<,>), typeof(NowValueResolver<,>));
			ServiceCollection.AddSingleton(typeof(IStringKeyValueResolver<,>), typeof(StringKeyValueResolver<,>));
		}

		private void AddSqlDataAccess()
		{
			ServiceCollection.AddDbContextPool<MyDbContext>((serviceProvider, builder) =>
			{
				var options = serviceProvider.GetRequiredService<IOptionsMonitor<PersistenceOptions>>().CurrentValue;
				builder.UseSqlServer(options.SqlDataStore.ConnectionString);
			});
			ServiceCollection.AddScoped<ITodoItemCommandDataAccess, TodoItemCommandDataAccess>();
		}
	}
}
