using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Provisioning.Service;

namespace DeviceAllocation
{
    public static class DeviceAllocationFn
    {
        [FunctionName("DeviceAllocationFn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
			string stringBody = null;
			using (var bodyReader = new StreamReader(req.Body))
			{
				stringBody = await bodyReader.ReadToEndAsync();
			}

			log.LogInformation("Request:");
			log.LogInformation(stringBody);

			var dynamicBody = JsonConvert.DeserializeObject(stringBody) as dynamic;

			var deviceRegistrationId = (string)dynamicBody?.deviceRuntimeContext?.registrationId;

			if (string.IsNullOrEmpty(deviceRegistrationId))
			{
				return new BadRequestObjectResult("Device id not found");
			}

			var iotHubHostNames = dynamicBody?.linkedHubs?.ToObject<string[]>();

			if (iotHubHostNames == null || iotHubHostNames.Length < 2)
			{
				return new BadRequestObjectResult("Expected at least 2 linked hubs");
			}

			string iotHubHostName = null;
			TwinCollection twinTags = new TwinCollection();
			TwinCollection twinProperties = new TwinCollection();

			if (deviceRegistrationId.StartsWith("Tata-"))
			{
				iotHubHostName = iotHubHostNames[0];
				twinTags["ServiceProvider"] = "Tata";
				twinProperties["channel"] = "1000";
			}
			else
			{
				iotHubHostName = iotHubHostNames[1];
				twinTags["ServiceProvider"] = "Airtel";
				twinProperties["channel"] = "1111";
			}

			var resultObject = new
			{
				iotHubHostName,
				initialTwin = new TwinState(twinTags, twinProperties)
			};

			log.LogInformation("Result:");
			log.LogInformation(JsonConvert.SerializeObject(resultObject));

			return new OkObjectResult(resultObject);
		}
    }
}
