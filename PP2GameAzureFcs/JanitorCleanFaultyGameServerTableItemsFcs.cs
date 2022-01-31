using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using PP2AzureConfig;
using PP2GameAzureFcs.GameServerDataFacade;

namespace PP2GameAzureFcs
{
    public class JanitorCleanFaultyGameServerTableItemsFcs
    {
        [FunctionName("JanitorCleanFaultyGameServerTableItemsAzFc")]
        public void Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            Task.Run(async () =>
            {
                await ProcessGameServerTable(log);
            });
        }

        private async Task ProcessGameServerTable(ILogger pLog)
        {
            var facade = new SqlGameServerDataFacade();

            var serversData = await facade.GetGameServerListAsync();

            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var cntsMap = azure.ContainerGroups.List().ToDictionary(k => int.Parse(k.Containers["unitygame"].EnvironmentVariables.FirstOrDefault(env => env.Name == "PP2_SERVER_ID").Value), v => v);

            var excp = cntsMap.Keys.Except(serversData.Select(sd => sd.id));

            foreach (var id in excp)
            {
                pLog.LogWarning($"Deleting unrelated container in table, name: pp2gameserver{id}");
                await azure.ContainerGroups.DeleteByResourceGroupAsync(PP2Config.ResourceGroupName, $"pp2gameserver{id}");
            }

            var faultyCnts = new List<IContainerGroup>();

            foreach (var cnt in azure.ContainerGroups.List())
            {
                if (cnt.Name.StartsWith("pp2gameserver") == false)
                {
                    faultyCnts.Add(cnt);
                    continue;
                }

                if (cnt.Containers.TryGetValue("unitygame", out var c) == false)
                {
                    faultyCnts.Add(cnt);
                    continue;
                }

                var envVar = GetEnvVarFromContainer(c, "PP2_SERVER_ID");
                if (envVar == null || int.TryParse(envVar.Value, out var dummy) == false)
                {
                    faultyCnts.Add(cnt);
                    continue;
                }
            }

            foreach (var cnt in faultyCnts)
            {
                pLog.LogWarning($"Deleting faulty container in '{PP2Config.ResourceGroupName}', name: {cnt.Name}");
                await azure.ContainerGroups.DeleteByResourceGroupAsync(PP2Config.ResourceGroupName, cnt.Name);
            }
        }

        private EnvironmentVariable GetEnvVarFromContainer(Container cnt, string envName)
        {
            var n = cnt.EnvironmentVariables.FirstOrDefault(env => env.Name == envName, null);
            return n;
        }
    }
}
