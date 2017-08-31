using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoorManWork;

namespace ConsoleApp
{
    public class DummyProductFactorLoader : AbstractProductFactorLoader
    {
        protected override IWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken)
        {
            if (Count > 1)
            {
                return null;
            }

            Interlocked.Increment(ref Count);

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var workItems = new [] {
                new DummyWorkItem(rand.Next()),
                new DummyWorkItem(rand.Next()),
                new DummyWorkItem(rand.Next())
            };
            Console.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IWorkItem>().ToArray();
        }
    }
}