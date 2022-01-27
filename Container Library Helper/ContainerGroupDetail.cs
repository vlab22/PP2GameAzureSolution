using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Container_Library_Helper
{
    public class ContainerGroupDetail
    {
        public string Name;
        public string RegionName;
        public string ResourceGroupName;
        public string State;
        public string Type;
        public string Fqdn;
        public int[] ExternalTcpPorts;
        public List<ContainerDetail> Containers;

        public ContainerGroupDetail()
        {

        }

        public ContainerGroupDetail(IContainerGroup cg)
        {
            Name = cg.Name;
            RegionName = cg.RegionName;
            ResourceGroupName = cg.ResourceGroupName;
            State = cg.State;
            Type = cg.Type;
            Fqdn = cg.Fqdn;
            ExternalTcpPorts = cg.ExternalTcpPorts;
            Containers = GetContainersDetails(cg);
        }

        List<ContainerDetail> GetContainersDetails(IContainerGroup cg)
        {
            var result = new List<ContainerDetail>();
            foreach (var container in cg.Containers)
            {
                result.Add(new ContainerDetail(container.Value));
            }

            return result;
        }
    }

    public struct ContainerDetail
    {
        public string Name;
        public IList<ContainerPort> Ports;
        public string Image;

        public ContainerDetail(Container c)
        {
            Name = c.Name;
            Ports = c.Ports;
            Image = c.Image;
        }
    }
}
