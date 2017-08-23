using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
