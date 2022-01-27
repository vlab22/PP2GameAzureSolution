using Container_Library_Helper;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP2GameAzureFcs
{
    internal class AzureContainerGroupHelper
    {
        internal static async Task<IContainerGroup> GetByNameAndResourceGroup(string rg, string name)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var cntGrp = await azure.ContainerGroups.GetByResourceGroupAsync(rg, name);
        
            return cntGrp;
        }

        internal static async Task<IEnumerable<ContainerGroupDetail>> GetContainerGroupsDetailsList(string resourceGroupName)
        {
            var azure = AzureFactory.AzureCreator.GetAzureContext();
            var cgsList = await azure.ContainerGroups.ListByResourceGroupAsync(resourceGroupName);

            return cgsList.Select(cnt => new ContainerGroupDetail(cnt));
        }
    }
}
