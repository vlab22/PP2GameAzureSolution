using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PP2GameAzureFcs.GameServerDataFacade;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Management.ContainerInstance.Fluent;

namespace PP2GameAzureFcs
{
    public static class ProcessNewConnectionsFcs
    {
        static ILogger _log;

        [FunctionName("ProcessNewConnectionFcAz")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _log = log;

            log.LogInformation($"Requested by {req.HttpContext?.Connection?.RemoteIpAddress}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            bool auth = AuthorizationHelper.CheckUserAndPass((string)data?.user_auth, (string)data?.user_pass_auth);

            if (!auth)
            {
                return AuthorizationHelper.ForbidIncorrectPostPassword();
            }

            //Get Servers and Process
            var gameServers = await GetGameServersDataAndCheckScaleUp(log);

            return new JsonResult(gameServers);
        }

        private static async Task<List<GameServerDetailData>> GetGameServersDataAndCheckScaleUp(ILogger pLog)
        {
            var gameServers = await GetGameServersData();

            var maxSlots = gameServers.Sum(gs => gs.maxPlayers);
            var playersCount = gameServers.Sum(gs => gs.playersCount);

            //Verify free slots
            int freeSlots = maxSlots - playersCount;
            if (freeSlots > 0)
            {
            }
            else
            {
                //Insert Data in GameServer Table
                await CreateNewGameServerContainer(pLog);
            }

            return gameServers;
        }

        private static async Task CreateNewGameServerContainer(ILogger pLog)
        {
            var facade = new SqlGameServerDataFacade();
            int serverId = await facade.InsertForIdAsync();

            string region = "westeurope";

            var serverDetails = new GameServerDetailData()
            {
                id = serverId,
                name = $"pp2gameserver{serverId}",
                dns =  $"pp2gameserver{serverId}",
                region = region,
                maxPlayers = 2,
                playersCount = 0,
                status = "Initializing"
            };

            int update = await facade.UpdateAndSetLastPortAsync(
                serverDetails.id
                , serverDetails.name
                , serverDetails.dns
                , serverDetails.region
                , serverDetails.maxPlayers
                , serverDetails.playersCount
                , serverDetails.status);

            var serverDetailsToCreate = await facade.GetGameServerByIdAsync(serverId);

            //Expand async, not wait
            CreateContainerAndValidate(serverDetailsToCreate, pLog);

        }

        static async void CreateContainerAndValidate(GameServerDetailData serverDetails, ILogger pLog)
        {

            pLog.LogWarning($"Trying creating container {serverDetails.name} | {serverDetails.port}");

            IContainerGroup cnt = null;

            try
            {
                cnt = await ContainerFactory.GameUnityServer.CreateLinuxAsync(
                        serverDetails.id,
                        serverDetails.name,
                        PP2AzureConfig.PP2Config.ResourceGroupName,
                        serverDetails.dns,
                        serverDetails.port,
                        serverDetails.region,
                        serverDetails.maxPlayers
                );
            }
            catch (Exception ex)
            {
                pLog.LogError(ex.Message + " | " + ex.StackTrace);
            }

            if (cnt == null)
            {
                pLog.LogWarning($"Container cnt result NULL for {serverDetails.name} | {serverDetails.port}");
            }
        }

        private static async Task<List<GameServerDetailData>> GetGameServersData()
        {
            var sqlConnect = new SqlGameServerDataFacade();
            return await sqlConnect.GetGameServerListAsync();
        }
    }


}
