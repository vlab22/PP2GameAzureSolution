using Container_Library_Helper;
using ContainerFactory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PP2AzureConfig;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    public static class CreateGameContainerAzFc
    {
        [FunctionName("CreateGameContainer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            bool auth = AuthorizationHelper.CheckUserAndPass((string)data?.user_auth, (string)data?.user_pass_auth);

            if (!auth)
            {
                return AuthorizationHelper.ForbidIncorrectPostPassword();
            }

            var azure = AzureFactory.AzureCreator.GetAzureContext();

            var cntGr = await ContainerFactory.GameUnityServer.CreateLinuxAsync("pp2gameinst1", PP2Config.ResourceGroupName, "pp2gameinst1", "westeurope", 55551);

            return new JsonResult(new ContainerGroupDetail(cntGr));
        }
    }
}
