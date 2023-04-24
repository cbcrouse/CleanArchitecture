using Application.Configuration;
using Application.Interfaces;
using Application.Interfaces.ValueResolvers;
using Application.Mapping;
using AutoMapper;
using Common.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.ValueResolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Persistence.Sql;
using Persistence.Sql.Configuration;
using Persistence.Sql.TodoItem;
using System.Collections.Generic;
using System.Linq;

// This namespace has been intentionally changed for convenience.
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static DataAnnotationsValidator Validator { get; }

        static ServiceCollectionExtensions()
        {
            Validator = new DataAnnotationsValidator();
        }

        /// <summary>
        /// This function registers generic configured options and returns the configured type.
        /// </summary>
        /// <typeparam name="T">The options' class type.</typeparam>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="servicesCollection">Specifies the contract for a collection of service descriptors.</param>
        public static void RegisterConfiguredOptions<T>(this IServiceCollection servicesCollection, IConfiguration configuration) where T : class, new()
        {
            string sectionKey = typeof(T).Name;
            IConfigurationSection section = configuration.GetSection(sectionKey.Replace("Options", ""));

            var options = new T();
            section.Bind(options);

            if (options is IValidatable validatableOptions)
            {
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                Validator.TryValidateObjectRecursive(validatableOptions, results);
                if (results.Any())
                {
                    List<ValidationFailure> failures = results
                        .Select(result => new ValidationFailure(result.MemberNames.First(), result.ErrorMessage))
                        .ToList();

                    throw new ValidationException(failures);
                }
            }

            servicesCollection.Configure<T>(section);
        }

        /// <summary>
        /// Extension method for registering AutoMapper in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// Registers an <see cref="IMapper"/> instance to provide type mapping between objects.
        /// </remarks>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> instance to register AutoMapper with.</param>
        /// <seealso cref="MapperConfigurationExpression"/>
        /// <seealso cref="ApplicationProfile"/>
        /// <seealso cref="NowValueResolver{TSource,TDestination}"/>
        /// <seealso cref="StringKeyValueResolver{TSource,TDestination}"/>
        /// <seealso cref="IMapper"/>
        public static void RegisterAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<Profile, ApplicationProfile>();
            serviceCollection.AddSingleton(serviceProvider =>
            {
                var configurationExpression = new MapperConfigurationExpression();
                var profiles = serviceProvider.GetServices<Profile>();
                foreach (Profile profile in profiles)
                {
                    configurationExpression.AddProfile(profile);
                }
                configurationExpression.ConstructServicesUsing(serviceProvider.GetService);
                var configuration = new MapperConfiguration(configurationExpression);
                return configuration.CreateMapper();
            });

            // AutoMapper Resolvers
            serviceCollection.AddSingleton(typeof(INowValueResolver<,>), typeof(NowValueResolver<,>));
            serviceCollection.AddSingleton(typeof(IStringKeyValueResolver<,>), typeof(StringKeyValueResolver<,>));
        }

        /// <summary>
        /// Registers services related to SQL data access in the specified <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> instance to add the services to.</param>
        public static void AddSqlDataAccess(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContextPool<MyDbContext>((serviceProvider, builder) =>
            {
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<PersistenceOptions>>().CurrentValue;
                builder.UseSqlServer(options.SqlDataStore.ConnectionString);
            });
            serviceCollection.AddScoped<ITodoItemCommandDataAccess, TodoItemCommandDataAccess>();
        }
    }
}