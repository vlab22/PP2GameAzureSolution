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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            bool auth = AuthorizationHelper.CheckUserAndPass((string)data?.user_auth, (string)data?.user_pass_auth);

            if (!auth)
            {
                return AuthorizationHelper.ForbidIncorrectPostPassword();
            }

            //Get Servers and Process
            var gameServers = await GetGameServersDataAndCheckScaleUp();

            return new JsonResult(gameServers);
        }

        private static async Task<List<GameServerDetailData>> GetGameServersDataAndCheckScaleUp()
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
                await CreateNewGameServerContainer();
            }

            return gameServers;
        }

        private static async Task CreateNewGameServerContainer()
        {
            var facade = new SqlGameServerDataFacade();
            int serverId = await facade.InsertForIdAsync();

            string region = "westeurope";

            var serverDetails = new GameServerDetailData()
            {
                id = serverId,
                name = $"pp2gameserver{serverId}",
                dns = $"pp2gameserver{serverId}",
                port = 55551,
                region = region,
                maxPlayers = 3,
                playersCount = 0,
                status = "Initializing"
            };

            int update = await facade.UpdateAsync(
                serverDetails.id
                , serverDetails.name
                , serverDetails.dns
                , serverDetails.port
                , serverDetails.region
                , serverDetails.maxPlayers
                , serverDetails.playersCount
                , serverDetails.status);


            //Expand async, not wait
            CreateContainerAndValidate(serverDetails);

        }

        static async void CreateContainerAndValidate(GameServerDetailData serverDetails)
        {
            try
            {
                var cnt = await ContainerFactory.GameUnityServer.CreateLinuxAsync(
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
                _log.LogError(ex.Message + " | " + ex.StackTrace);
            }
        }

        private static async Task<List<GameServerDetailData>> GetGameServersData()
        {
            var sqlConnect = new SqlGameServerDataFacade();
            return await sqlConnect.GetGameServerListAsync();
        }
    }


}
