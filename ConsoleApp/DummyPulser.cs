using System;
using System.Threading;

namespace ConsoleApp
{
    public class DummyPulser
    {
        public static Thread Create(EventWaitHandle workArrived, Action updateStateAction, CancellationToken cancellationToken)
        {
            var emitter = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Dummy Pulser: hearthbeat...");
                    updateStateAction();
                    workArrived.Set();
                    cancellationToken.WaitHandle.WaitOne(10000);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });

            return emitter;
        }
    }
}