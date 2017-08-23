using System;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var sdpfl = new SyncedDummyProductFactorLoader())
            {
                sdpfl.Start();
                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
