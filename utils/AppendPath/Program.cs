using System;

namespace AppendPath
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("You must specify the name of the path you would like to append");
                return;
            }
            var path = Environment.GetEnvironmentVariable("PATH");
            Environment.SetEnvironmentVariable("PATH", path + ";" + args[0], EnvironmentVariableTarget.Machine);
        }
    }
}
