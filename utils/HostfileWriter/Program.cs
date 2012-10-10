using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace HostfileWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var role in RoleEnvironment.Roles.Values)
            {
                foreach (var instance in role.Instances.Where(x => x.InstanceEndpoints.Count > 0))
                {
                    Console.WriteLine(string.Format("{0} {1}", instance.InstanceEndpoints.Values.First().IPEndpoint.Address, instance.Id));

                }
            }
        }
    }
}
