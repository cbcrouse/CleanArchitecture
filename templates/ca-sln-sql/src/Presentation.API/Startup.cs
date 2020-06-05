using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using HealthChecks.UI.Client;
using Infrastructure.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Persistence.Sql;
using Presentation.API.Configuration;
using Presentation.API.Mapping;

namespace Presentation.API
{
	/// <summary>
	/// Default startup for the _APP_NAME_ API.
	/// </summary>
	public class Startup : PresentationStartupOrchestrator<AppStartupOrchestrator>
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public Startup()
		{
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<SwaggerOptions>(Configuration));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<HealthChecksUIOptions>(Configuration));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IHttpContextAccessor), typeof(HttpContextAccessor)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IActionContextAccessor), typeof(ActionContextAccessor)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddCors());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddAuthentication());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddAuthorization());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddHealthChecks());
			ServiceRegistrationExpressions.Add(() => AddMvcCore());
			ServiceRegistrationExpressions.Add(() => AddSwagger());
			ServiceRegistrationExpressions.Add(() => AddHealthChecks());

			MapperExtensionExpressions.Add(mapperConfig => mapperConfig.AddProfile(typeof(PresentationProfile)));
		}

		private void AddMvcCore()
		{
			var builder = ServiceCollection.AddMvcCore(options => { options.RequireHttpsPermanent = true; });

			builder.AddApiExplorer();
			builder.AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				options.SerializerSettings.Converters.Add(new StringEnumConverter());
			});
			builder.AddMvcOptions(options =>
			{
				options.Conventions.Add(new Authorization.AuthorizeControllerModelConvention());
			});
		}

		private void AddSwagger()
		{
			var swaggerOptions = ServiceCollection.BuildServiceProvider().GetRequiredService<IOptionsMonitor<SwaggerOptions>>().CurrentValue;

			if (swaggerOptions.IsEnabled)
			{
				ServiceCollection.AddSwaggerGen(options =>
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
		/// Adds healthcheck functionality to the <see cref="IServiceCollection"/>.
		/// </summary>
		protected virtual void AddHealthChecks()
		{
			ServiceCollection.Configure<HealthCheckPublisherOptions>(options =>
			{
				options.Delay = TimeSpan.FromSeconds(2);
				options.Predicate = check => check.Tags.Contains("db_tag");
			});

			ServiceCollection.AddSingleton<IHealthCheckPublisher, HealthChecks.ReadinessPublisher>();
			IHealthChecksBuilder builder = ServiceCollection
				.AddHealthChecks()
				.AddDbContextCheck<MyDbContext>("My Database Context", tags: new[] { "db_tag" });
		}

		///<summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		///</summary>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

			// The order of these executions matters except when specified.
			UseDeveloperExceptionPage(app);
			var swaggerOptions = app.ApplicationServices.GetRequiredService<IOptionsMonitor<SwaggerOptions>>().CurrentValue;
			if (swaggerOptions.IsEnabled)
			{
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					string relativeUrl = $"/swagger/v{fvi.FileVersion}/swagger.json";
					options.SwaggerEndpoint(relativeUrl, swaggerOptions.DropdownDescription);
				});
			}

			app.UseRouting();
			UseCors(app);
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseHttpsRedirection();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
				endpoints
					.MapHealthChecks("health", new HealthCheckOptions
					{
						Predicate = _ => true,
						ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
					});

				var healthOptions = app.ApplicationServices.GetRequiredService<IOptionsMonitor<HealthChecksUIOptions>>().CurrentValue;
				if (healthOptions.IsEnabled)
				{
					// endpoint is /healthchecks-ui
					endpoints.MapHealthChecksUI();
				}
			});
		}

		private void UseDeveloperExceptionPage(IApplicationBuilder app)
		{
			var env = app.ApplicationServices.GetService<IHostEnvironment>();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				IdentityModelEventSource.ShowPII = true;
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
		}

		private void UseCors(IApplicationBuilder app)
		{
			app.UseCors(policyBuilder =>
			{
				policyBuilder.AllowAnyHeader();
				policyBuilder.AllowAnyMethod();
				policyBuilder.AllowAnyOrigin();
			});
		}
	}
}
