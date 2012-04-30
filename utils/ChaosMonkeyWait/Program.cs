using System;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ChaosMonkeyWait
{
    class Program
    {
        static void Main(string[] args)
        {
            var value = RoleEnvironment.GetConfigurationSettingValue("Two10.ChaosMonkey.AverageHoursBeforeReboot");
            int hours = 0;
            if (null != value && int.TryParse(value, out hours))
            {
                var rand = new Random();
                var wait = TimeSpan.FromHours(hours).TotalMinutes * rand.Next() * 2;
                Thread.Sleep(TimeSpan.FromMinutes(wait));
            }
            else
            {
                // sleep forever
                Thread.Sleep(-1);
            }
        }
    }
}
