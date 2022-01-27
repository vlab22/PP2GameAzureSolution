using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Container_Library_Helper;
using Microsoft.Azure.Management.Fluent;
using System.Linq;
using Microsoft.Rest.Azure;
using System.Collections.Generic;
using PP2AzureConfig;

namespace PP2GameAzureFcs
{
    public static class DestroyContainer
    {
        [FunctionName("DestroyContainer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            bool auth = AuthorizationHelper.CheckUserAndPass((string)(data?.user), (string)(data?.pass));

            if (!auth || data?.name == null)
            {
                return new ForbidResult();
            }

           string result = await DestroyContainerGroupByNameAsync((string)(data?.name), log);

            return new JsonResult(result == "" ? new OkObjectResult($"ContainerGroup {data?.name} destroyed") : new NotFoundObjectResult(result));
        }

        private static async Task<string> DestroyContainerGroupByNameAsync(string name, ILogger log)
        {
            IAzure azure;

            try
            {
                azure = AzureFactory.AzureCreator.GetAzureContext();

                var cntGrp = await AzureContainerGroupHelper.GetByNameAndResourceGroup(PP2Config.ResourceGroupName, name);
                if (cntGrp == null) 
                    return "container not found";

                await azure.ContainerGroups.DeleteByResourceGroupAsync(PP2Config.ResourceGroupName, name);

                log.LogWarning($"Container Group name: {name} in resource group: {PP2Config.ResourceGroupName} DESTROYED");

                return "";
            }
            catch (CloudException ex)
            {
                log.LogWarning($"Container Group name: {name} not found in resource group: {PP2Config.ResourceGroupName} | {ex.Message}");
                return ex.Message;
            }
        }
    }
}
