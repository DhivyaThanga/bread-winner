using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoorManWork;

namespace ConsoleApp
{
    public class SyncedDummyProductFactorLoader : AbstractProductFactorLoader
    {
        protected override IPoorManWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken)
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
            var synchronizer = new PoorManWorkBatchSynchronizer(3);
            var workItems = new [] {
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };
            Console.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IPoorManWorkItem>().ToArray();
        }
    }
}