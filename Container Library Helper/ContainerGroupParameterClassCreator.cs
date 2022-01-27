using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Container_Library_Helper
{
    public class ContainerGroupParameterClassCreator
    {
        public static string Run()
        {
            var json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\parameters.json");
            var obj = JsonConvert.DeserializeObject(json);

            return "";
        }
    }
}
