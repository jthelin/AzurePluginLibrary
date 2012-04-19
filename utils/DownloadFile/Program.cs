using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DownloadFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("please specify the URL and the local filename as the first two arguments");
                return;
            }
            try
            {
                var client = new WebClient();
                client.DownloadFile(args[0], args[1]);
                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.ExitCode = -1;
            }
        }
    }
}
