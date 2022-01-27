using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.ContainerGroup.Definition;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;

namespace ContainerFactory
{
    public class GameUnityServer
    {
        private static IWithCreate CreateLinuxDefinition(string name, string resourceGroup, string dns, string region, int port, IAzure azure)
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
                    .Attach()
                .WithRestartPolicy(Microsoft.Azure.Management.ContainerInstance.Fluent.Models.ContainerGroupRestartPolicy.OnFailure)
                .WithDnsPrefix(dns);

            return iWith;
        }

        public static async Task<IContainerGroup> CreateLinuxAsync(string name, string resourceGroup, string dns, string region, int port)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var iWith = CreateLinuxDefinition(name, resourceGroup, dns, region, port, azure);
            var cnt = await iWith.CreateAsync();

            return cnt;
        }

        public static IContainerGroup CreateLinux(string name, string resourceGroup, string dns, string region, int port)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var iWith = CreateLinuxDefinition(name, resourceGroup, dns, region, port, azure);
            var cnt =  iWith.Create();

            return cnt;
        }

    }
}