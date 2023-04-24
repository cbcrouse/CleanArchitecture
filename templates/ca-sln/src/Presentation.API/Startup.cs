using HealthChecks.UI.Client;
using Infrastructure.Extensions;
using Infrastructure.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Presentation.API.Configuration;
using StartupOrchestration.NET;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Presentation.API
{
    /// <summary>
    /// Default startup for the _APP_NAME_ API.
    /// </summary>
    public class Startup : StartupOrchestrator<AppStartupOrchestrator>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
        {
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<SwaggerOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.RegisterConfiguredOptions<HealthChecksUIOptions>(config));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IHttpContextAccessor), typeof(HttpContextAccessor)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddTransient(typeof(IActionContextAccessor), typeof(ActionContextAccessor)));
            ServiceRegistrationExpressions.Add((services, config) => services.AddCors());
            ServiceRegistrationExpressions.Add((services, config) => services.AddAuthentication());
            ServiceRegistrationExpressions.Add((services, config) => services.AddAuthorization());
            ServiceRegistrationExpressions.Add((services, config) => services.AddHealthChecks());
            ServiceRegistrationExpressions.Add((services, config) => services.AddMvcCoreCustom());
            ServiceRegistrationExpressions.Add((services, config) => services.AddSwagger(config));
        }

        /// <inheritdoc />
        protected override void AddConfigurationProviders(IConfigurationBuilder builder)
        {
            builder.AddPrioritizedSettings();
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
                    endpoints.MapHealthChecksUI();
                }
            });
        }

        private void UseDeveloperExceptionPage(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IHostEnvironment>();
            if (env != null && env.IsDevelopment())
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
