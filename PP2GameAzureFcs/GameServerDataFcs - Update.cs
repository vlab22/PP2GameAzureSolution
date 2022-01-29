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

namespace PP2GameAzureFcs
{
    public static partial class GameServerDataFcs
    {
        [FunctionName("UpdateGameServerPlayerCountDataAzFc")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            bool auth = AuthorizationHelper.CheckUserAndPass((string)(data?.user_auth), (string)(data?.user_pass_auth));

            if (!auth)
            {
                return AuthorizationHelper.ForbidIncorrectPostPassword();
            }

            int id = await UpdateGameServerPlayerCount(data);

            return new OkObjectResult(id);
        }

        private static async Task<int> UpdateGameServerPlayerCount(dynamic data)
        {
            if (int.TryParse((string)data?.player_count, out int playerCount) == false
                || int.TryParse((string)data?.server_id, out int serverId) == false)
            {
                return -1;
            }

            var saver = new SqlGameServerDataFacade();

            int id = await saver.UpdateAsync(serverId, playerCount);

            return id;
        }
    }
}
