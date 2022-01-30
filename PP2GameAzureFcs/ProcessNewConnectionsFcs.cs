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

namespace PP2GameAzureFcs
{
    public static class ProcessNewConnectionsFcs
    {
        [FunctionName("ProcessNewConnectionFcAz")]
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

            //Get Servers and Process
            var gameServers = await GetGameServersDataAndCheckScaleUp();

            return new JsonResult(gameServers);
        }

        private static async Task<List<GameServerDetailData>> GetGameServersDataAndCheckScaleUp()
        {
            var gameServers = await GetGameServersData();

            var maxSlots = gameServers.Sum(gs => gs.maxPlayers);
            var playersCount = gameServers.Sum(gs => gs.playerCount);

            //Verify free slots
            int freeSlots = maxSlots - playersCount;
            if (freeSlots > 0)
            {
            }
            else
            {
                //Expand
            }

            return gameServers;
        }

        private static async Task<List<GameServerDetailData>> GetGameServersData()
        {
            var sqlConnect = new SqlGameServerDataFacade();
            return await sqlConnect.GetGameServerListAsync();
        }
    }


}
