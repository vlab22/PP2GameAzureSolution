using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    internal class AuthorizationHelper
    {
        const string user = "vlab22pp2";
        const string password = "password";

        public static async Task<bool> CheckRequestBodyUserAndPass(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var result = data?.user == user && data?.pass == password;

            return result;
        }

        public static bool CheckUserAndPass(string pUser, string pPass)
        {
            var result = pUser == user && pPass == password;

            return result;
        }
    }
}
