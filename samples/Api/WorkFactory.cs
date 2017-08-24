using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PoorManWork;
using PoorManWorkManager;

namespace Api
{
    public class WorkFactory : IPoorManWorkFactory
    {
        private int _count;

        public void Reset()
        {
            Interlocked.Exchange(ref _count, 0);
        }

        public IPoorManWorkItem[] Create(CancellationToken cancellationToken)
        {
            if (_count > 1)
            {
                return null;
            }

            Interlocked.Increment(ref _count);

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var synchronizer = new PoorManWorkBatchSynchronizer(3);
            var workItems = new[]
            {
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };

            Debug.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IPoorManWorkItem>().ToArray();
        }
    }
}