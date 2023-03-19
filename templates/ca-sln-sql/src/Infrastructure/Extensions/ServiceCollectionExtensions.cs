using Application.Configuration;
using Common.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

// This namespace has been intentionally changed for convenience.
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extensions for <see cref="IConfiguration"/>.
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
            section ??= configuration.GetSection(sectionKey);

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
    }
}