using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public class SyncedDummyWorkItem : PoorManSyncedWorkItem
    {
        public SyncedDummyWorkItem(
            int id, PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken) 
            : base(id.ToString(), workBatchSynchronizer, cancellationToken)
        {
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");
            cancellationToken.WaitHandle.WaitOne(2000);
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId}, consuming {Id}, LAST OF THE BATCH");
        }
    }
}