using System;
using System.Diagnostics;
using System.Threading;
using SamplesShared;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());

            var pool = WorkerPoolExample.CreatePool(
                new TimeSpan(0, 0, 0, 15),
                new TimeSpan(0, 0, 0, 10),
                100);
            var tokenSource = new CancellationTokenSource();
            pool.Start(tokenSource.Token);

            Console.ReadKey();
            
            tokenSource.Cancel(false);

            Console.ReadKey();
        }
    }
}
