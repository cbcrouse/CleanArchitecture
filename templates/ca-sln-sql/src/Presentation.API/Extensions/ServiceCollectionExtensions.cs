using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Persistence.Sql;
using Presentation.API.Authorization;
using Presentation.API.Configuration;
using Presentation.API.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

// This namespace has been intentionally changed for convenience.
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers essential services required for using ASP.NET Core MVC with custom options.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to register services with.</param>
    /// <remarks>
    /// This method registers essential services required for using ASP.NET Core MVC with custom options. It adds the following services to the specified <paramref name="serviceCollection"/>:
    /// <list type="bullet">
    ///     <item><description>MvcCore services with HTTPS enabled.</description></item>
    ///     <item><description>API Explorer services.</description></item>
    ///     <item><description>NewtonsoftJson services with default options that include CamelCasePropertyNamesContractResolver and StringEnumConverter.</description></item>
    ///     <item><description>MvcOptions with a custom AuthorizeControllerModelConvention.</description></item>
    /// </list>
    /// </remarks>
    public static void AddMvcCoreCustom(this IServiceCollection serviceCollection)
    {
        var builder = serviceCollection.AddMvcCore(options => { options.RequireHttpsPermanent = true; });

        builder.AddApiExplorer();
        builder.AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
        builder.AddMvcOptions(options =>
        {
            options.Conventions.Add(new AuthorizeControllerModelConvention());
        });
    }

    /// <summary>
    /// Adds Swagger support to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add Swagger to.</param>
    /// <param name="configuration"></param>
    /// <remarks>
    /// The Swagger documentation is generated based on the XML comments in the assembly where this method is called.
    /// The documentation file is determined automatically based on the name of the calling assembly and the extension ".xml".
    /// 
    /// This method also configures Swagger to include the XML comments from controller classes, based on the "IncludeControllerXmlComments"
    /// setting in the "SwaggerOptions" configuration section.
    /// 
    /// In addition, this method configures Swagger to use the JWT token in the "Authorization" header as the bearer token for authentication,
    /// by adding the "Bearer" security definition and requirement.
    /// </remarks>
    public static void AddSwagger(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var swaggerOptions = new SwaggerOptions();
        configuration.GetSection("Swagger").Bind(swaggerOptions);

        if (swaggerOptions.IsEnabled)
        {
            serviceCollection.AddSwaggerGen(options =>
            {
                var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                string version = $"v{fvi.FileVersion}";
                options.SwaggerDoc(version, new OpenApiInfo { Title = swaggerOptions.Title, Version = version });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, swaggerOptions.IncludeControllerXmlComments);

                var bearerDef = new OpenApiSecurityScheme
                {
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header
                    //Flows = new OpenApiOAuthFlows(){}
                };
                options.AddSecurityDefinition("Bearer", bearerDef);

                var bearerReq = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                };
                options.AddSecurityRequirement(bearerReq);

                // TODO: Setup operation filters.
            });
        }
    }

    /// <summary>
    /// Registers health checks for the service, including a custom readiness check publisher and a database context check.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public static void AddHealthChecksCustom(this IServiceCollection serviceCollection)
    {
        serviceCollection.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(2);
            options.Predicate = check => check.Tags.Contains("db_tag");
        });

        serviceCollection.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();
        IHealthChecksBuilder builder = serviceCollection
            .AddHealthChecks()
            .AddDbContextCheck<MyDbContext>("My Database Context", tags: new[] { "db_tag" });
    }
}