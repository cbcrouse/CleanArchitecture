using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Presentation.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog((context, config) =>
				{
					// Setup Serilog: https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
					// Configuration: https://github.com/serilog/serilog-settings-configuration
					// Sinks        : https://github.com/serilog/serilog/wiki/Provided-Sinks
					// Wiki         : https://github.com/serilog/serilog/wiki
					config.ReadFrom.Configuration(context.Configuration);
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
