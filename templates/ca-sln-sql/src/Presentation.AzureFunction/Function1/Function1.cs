using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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
		/// <param name="log"></param>
		[FunctionName("Function1")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			string name = req.Query["name"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);
			name ??= data.name;

			string responseMessage = string.IsNullOrEmpty(name)
				? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
				: $"Hello, {name}. This HTTP triggered function executed successfully.";

			return new OkObjectResult(responseMessage);
		}
	}
}
