using System;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ExecuteMultipleCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please specify the command and the setting name containing the arguments");
            }

            string command = args[0];
            string setting = args[1];

            var value = RoleEnvironment.GetConfigurationSettingValue(setting);
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            var values = value.Split(',', ';');
            foreach (var item in values)
            {
                Console.WriteLine("{0} {1}", command, item.Trim());
            }

        }
    }
}
