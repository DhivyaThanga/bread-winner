using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class DummyPulser
    {
        public static Task Create(EventWaitHandle workArrived, Action updateStateAction, CancellationToken cancellationToken)
        {
            var pulser = new Task(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Dummy Pulser: hearthbeat...");
                    updateStateAction();
                    workArrived.Set();
                    cancellationToken.WaitHandle.WaitOne(10000);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning);

            return pulser;
        }
    }
}