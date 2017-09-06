using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using SamplesShared;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            var tokenSource = new CancellationTokenSource();

            WorkerPoolExample.StartPool(
                false,
                new TimeSpan(0, 0, 2, 0),
                new TimeSpan(0, 0, 0, 10),
                100,
                tokenSource.Token);

            CloudConsole.WriteLine("Pool started correctly!");

            while (Console.ReadKey().KeyChar != 'q')
            {
            }

            tokenSource.Cancel();
        }
    }
}
