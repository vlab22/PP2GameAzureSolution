using Container_Library_Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PP2AzureConfig;
using System.IO;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    public static class CreateGameContainerAzFc
    {
        [FunctionName("CreateGameContainer")]
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

            var test = ContainerGroupParameterClassCreator.Run();

            var cntGrpsDetail = await AzureContainerGroupHelper.GetContainerGroupsDetailsList(PP2Config.ResourceGroupName);

            return new JsonResult(cntGrpsDetail);
        }
    }
}
