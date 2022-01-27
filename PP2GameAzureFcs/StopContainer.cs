using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Container_Library_Helper;
using Microsoft.Rest.Azure;
using PP2AzureConfig;

namespace PP2GameAzureFcs
{
    public static class StopContainer
    {
        [FunctionName("StopContainer")]
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

            IContainerGroup cntStopped = await StopContainerGroupByNameAsync(data?.name, log);

            return new JsonResult(cntStopped != null ? new ContainerGroupDetail(cntStopped) : new NotFoundObjectResult(""));
        }

        private static async Task<IContainerGroup> StopContainerGroupByNameAsync(string name, ILogger log)
        {
            IAzure azure;

            try
            {
                azure = AzureFactory.AzureCreator.GetAzureContext();
                var cntGrp = azure.ContainerGroups.GetByResourceGroup(PP2Config.ResourceGroupName, name);
                if (cntGrp == null)
                    return null;

                await cntGrp.StopAsync();

                return cntGrp;
            }catch (CloudException ex)
            {
                log.LogWarning(ex.Message + " | " + ex.StackTrace);
                return null;
            }
        }
    }
}
