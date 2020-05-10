using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Startup
{
    /// <summary>
    /// This class is designed to provide a way to orchestrate dependency registrations while being
    /// presentation agnostic. The presentation layer connects to this orchestrator through generic inheritance
    /// in order to pipe in presentation-specific dependencies as well as provide the <see cref="IServiceCollection"/>
    /// and <see cref="IConfiguration"/>.
    /// </summary>
    public abstract class CoreStartupOrchestrator
    {
        /// <summary>
        /// This function compiles and invokes the <see cref="ServiceRegistrationExpressions"/>.
        /// </summary>
        public void Orchestrate()
        {
            AugmentExpressionExecutions(ServiceRegistrationExpressions);
        }

        /// <summary>
        /// This function allows the presentation layer to send the <see cref="IServiceCollection"/>
        /// down to the infrastructure layer so the Application can take control of the service registrations.
        /// </summary>
        /// <param name="collection">Specifies the contract for a collection of service descriptors.</param>
        public IServiceCollection InitializeServiceCollection(IServiceCollection collection)
        {
            ServiceCollection = collection;
            return ServiceCollection;
        }

        /// <summary>
        /// This function allows the presentation layer to send the <see cref="IConfiguration"/>
        /// down to the infrastructure layer so the Application can take control of loading configurations.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        public IConfiguration InitializeConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
            return Configuration;
        }

        /// <summary>
        /// Add additional service registration expressions.
        /// </summary>
        /// <param name="expressions">A list of expressions that do not require configured options.</param>
        public void AddServiceRegistrationExpressions(IList<Expression<Action>> expressions)
        {
            foreach (Expression<Action> expression in expressions)
            {
                ServiceRegistrationExpressions.Add(expression);
            }
        }

        /// <summary>
        /// Add to a list of AutoMapper configuration expressions that will be invoked by the implementor.
        /// </summary>
        /// <param name="expressions">The expressions to invoke after service registrations.</param>
        public void AddMapperExtensionExpressions(IList<Expression<Action<IMapperConfigurationExpression>>> expressions)
        {
            foreach (Expression<Action<IMapperConfigurationExpression>> expression in expressions)
            {
                MapperExtensionExpressions.Add(expression);
            }
        }

        /// <summary>
        /// A contract for a collection of service descriptors.
        /// </summary>
        protected IServiceCollection ServiceCollection { get; private set; } = new ServiceCollection();

        /// <summary>
        /// A set of key/value application configuration properties.
        /// </summary>
        protected IConfiguration Configuration { get; set; } = new ConfigurationBuilder().Build();

        /// <summary>
        /// Represents a type used to configure the logging system and create instances of <see cref="T:Microsoft.Extensions.Logging.ILogger" />
        /// from the registered <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />s.
        /// </summary>
        protected virtual ILogger Logger => new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Verbose()
            .WriteTo.ApplicationInsights(new TraceTelemetryConverter(), LogEventLevel.Information)
            .WriteTo.Console(outputTemplate:"[{Timestamp:yyyy:MM:dd hh:mm:ss.fff tt}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
            .CreateLogger();

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

        /// <summary>
        /// Augments a collection of <see cref="Action"/> expressions.
        /// </summary>
        protected void AugmentExpressionExecutions(IEnumerable<Expression<Action>> expressions)
        {
            var expList = expressions.ToList();
            foreach (Expression<Action> expression in expList)
            {
                AugmentExpressionExecution(expression);
            }
        }

        /// <summary>
        /// Augments an <see cref="Action"/> expression.
        /// </summary>
        protected void AugmentExpressionExecution(Expression<Action> expression)
        {
            var expressionAsString = expression.Body.ToString();

            try
            {
                Logger.Verbose("'{Expression}' was started...", expressionAsString);
                expression.Compile().Invoke();
                Logger.Verbose("'{Expression}' completed successfully!", expressionAsString);
            }
            catch (Exception e)
            {
                Logger.Verbose(e, "'{Expression}' failed with an unhandled exception.", expression.Body.ToString());
                throw;
            }
        }

        /// <summary>
        /// Augments a collection of <see cref="Action{T}"/> expressions.
        /// </summary>
        protected void AugmentExpressionExecutions<T>(IEnumerable<Expression<Action<T>>> expressions, T parameter)
        {
            var expList = expressions.ToList();
            foreach (Expression<Action<T>> expression in expList)
            {
                AugmentExpressionExecution(expression, parameter);
            }
        }

        /// <summary>
        /// Augments an <see cref="Action{T}"/> expression.
        /// </summary>
        protected void AugmentExpressionExecution<T>(Expression<Action<T>> expression, T parameter)
        {
            var expressionAsString = expression.Body.ToString();

            try
            {
                Logger.Verbose("'{Expression}' was started...", expressionAsString);
                expression.Compile().Invoke(parameter);
                Logger.Verbose("'{Expression}' completed successfully!", expressionAsString);
            }
            catch (Exception e)
            {
                Logger.Verbose(e, "'{Expression}' failed with an unhandled exception.", expression.Body.ToString());
                throw;
            }
        }
    }
}
