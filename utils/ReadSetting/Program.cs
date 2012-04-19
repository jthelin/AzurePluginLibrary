using System;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ReadSetting
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("You must specify the name of the setting to retrieve");
                return;
            }
            Console.WriteLine(RoleEnvironment.GetConfigurationSettingValue(args[0]));
        }
    }
}
