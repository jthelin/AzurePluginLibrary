using System;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ChaosMonkeyWait
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var value = RoleEnvironment.GetConfigurationSettingValue("Two10.ChaosMonkey.AverageHoursBeforeReboot");
                int hours = 0;
                if (null != value && int.TryParse(value, out hours))
                {
                    var rand = new Random();
                    var wait = TimeSpan.FromHours(hours).TotalMinutes * rand.NextDouble() * 2;
                    Thread.Sleep(TimeSpan.FromMinutes(wait));
                }
                else
                {
                    // sleep forever
                    Thread.Sleep(-1);
                }
            }
            catch
            {
                Thread.Sleep(-1);
            }

        }
    }
}
