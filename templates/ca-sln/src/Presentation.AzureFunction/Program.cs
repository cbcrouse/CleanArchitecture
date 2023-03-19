using Microsoft.Extensions.Hosting;
using Presentation.AzureFunction.Startup;

var builder = CreateHostBuilder().Build();
// https://codetraveler.io/2021/02/12/creating-azure-functions-using-net-5/
// https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide
// https://github.com/Azure/azure-functions-core-tools
await builder.RunAsync();

IHostBuilder CreateHostBuilder() =>
    new HostBuilder()
        .ConfigureServices(services =>
        {
            var startup = new Startup();
            startup.ConfigureServices(services);
        })
        .ConfigureFunctionsWorkerDefaults(
            //workerApp =>
            //{
            //    // Azure Functions now support Middleware!
            //    // https://github.com/Azure/azure-functions-dotnet-worker/tree/main/samples/CustomMiddleware
            //    //workerApp.UseMiddleware()
            //}
        );