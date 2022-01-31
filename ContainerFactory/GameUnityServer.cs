using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.ContainerGroup.Definition;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;

namespace ContainerFactory
{
    public class GameUnityServer
    {
        private static IWithCreate CreateLinuxDefinition(int serverId, string name, string resourceGroup, string dns, int port
            , string region, int maxPlayers, IAzure azure)
        {
            var iWith = azure.ContainerGroups.Define(name)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroup)
                .WithLinux()
                .WithPrivateImageRegistry("pp2game.azurecr.io", "pp2game", "28HzGS7Oj48Fi1umstNK1mupKdi=SVEt")
                .WithoutVolume()
                .DefineContainerInstance("unitygame")
                    .WithImage("pp2game.azurecr.io/ubuntu-unity-game-server:latest")
                    .WithExternalTcpPort(port)
                    .WithMemorySizeInGB(1.5)
                    .WithCpuCoreCount(1)
                    .WithEnvironmentVariables(new Dictionary<string, string>{
                        { "PP2_SERVER_ID", serverId.ToString() },
                        { "PP2_SERVER_PORT", port.ToString() },
                        { "PP2_MAX_PLAYERS", maxPlayers.ToString() },
                        { "PLAYERS_COUNT", "0" }
                     })
                    .Attach()
                .WithRestartPolicy(Microsoft.Azure.Management.ContainerInstance.Fluent.Models.ContainerGroupRestartPolicy.OnFailure)
                .WithDnsPrefix(dns);

            return iWith;
        }

        public static async Task<IContainerGroup> CreateLinuxAsync(int serverId, string name, string resourceGroup, string dns, int port
            , string region, int maxPlayers)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var iWith = CreateLinuxDefinition(serverId, name, resourceGroup, dns, port, region, maxPlayers, azure);

            IContainerGroup cnt = null;

            try
            {
                cnt = await iWith.CreateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return cnt;
        }

        public static IContainerGroup CreateLinux(int serverId, string name, string resourceGroup, string dns, int port, string region, int maxPlayers)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var iWith = CreateLinuxDefinition(serverId, name, resourceGroup, dns, port, region, maxPlayers, azure);
            var cnt = iWith.Create();

            return cnt;
        }

    }
}