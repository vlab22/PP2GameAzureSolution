using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    internal class AuthorizationHelper
    {
        const string user = "pp2game";
        const string password = "KDcx6kB2v77B5bm5AwY7XakYSsSB7Q4R";

        public static async Task<bool> CheckUserPassAsync(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var result = data?.user_auth == user && data?.user_pass_auth == password;

            return result;
        }

        public static bool CheckUserAndPass(string pUser, string pPass)
        {
            var result = pUser == user && pPass == password;

            return result;
        }

        public static ObjectResult ForbidIncorrectPostPassword()
        {
            return new ObjectResult("Error 403 Forbidden")
            {
                StatusCode = (int?)HttpStatusCode.Forbidden
            };
        }
    }
}
