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
            var key = args[0];
            var value = RoleEnvironment.GetConfigurationSettingValue(key);
            try
            {
                Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Machine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Need elevation to set machine variable");
            }
            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
        }
    }
}
