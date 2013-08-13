using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;

namespace HostfileWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sw = new StreamWriter(@"hosts"))
            foreach (var role in RoleEnvironment.Roles.Values)
            {
                foreach (var instance in role.Instances.Where(x => x.InstanceEndpoints.Count > 0))
                {
                    sw.WriteLine(string.Format("{0} {1}", instance.InstanceEndpoints.Values.First().IPEndpoint.Address, instance.Id));
                    Console.WriteLine(string.Format("{0} {1}", instance.InstanceEndpoints.Values.First().IPEndpoint.Address, instance.Id));
                }
            }
        }
    }
}
