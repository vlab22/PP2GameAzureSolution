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
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace PP2GameAzureFcs
{
    public static class GetContainersAzFc
    {

        [FunctionName("GetContainersAzFc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            bool auth = await AuthorizationHelper.CheckUserPassAsync(req);

            if (!auth)
            {
                return AuthorizationHelper.ForbidIncorrectPostPassword();
            }

            JsonResult result = GetContainersListJson();
            return result;
        }

        private static JsonResult GetContainersListJson()
        {
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
