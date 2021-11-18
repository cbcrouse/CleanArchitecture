using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Presentation.AzureFunction.Function1
{
    /// <summary>
    /// 
    /// </summary>
    public static class Function1
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite#install-and-run-the-azurite-docker-image
        /// </summary>
        /// <param name="req"></param>
        /// <param name="context"></param>
        [Function(nameof(Function1))]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(Function1));
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to .NET 6!");

            return response;
        }
    }
}
