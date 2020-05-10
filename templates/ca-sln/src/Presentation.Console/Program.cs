using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace Presentation.Console
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = CreateHostBuilder(args).Build();

			System.Console.Write("Please enter a value: ");
			var read = System.Console.ReadLine();
			System.Console.WriteLine($"Your value: {read}");

			await builder.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices(services =>
				{
					var startup = new Startup();
					startup.ConfigureServices(services);
				});
	}
}
