using System;
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
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using Container_Library_Helper;

namespace PP2GameAzureFcs
{
    public static class FcGetContainers
    {

        [FunctionName("FcGetContainers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");


            bool auth = await AuthorizationHelper.CheckRequestBodyUserAndPass(req);

            if (!auth)
            {
                return new ForbidResult();
            }

            var azure = AzureFactory.AzureCreator.GetAzureContext();

            var cnts = azure.ContainerGroups.List();

            var containerGroupsDetails = new List<ContainerGroupDetail>();

            foreach (var cnt in cnts)
            {
                containerGroupsDetails.Add(new ContainerGroupDetail(cnt));
            }

            var result = new JsonResult(containerGroupsDetails);

            return result;
        }


        
    }
}
