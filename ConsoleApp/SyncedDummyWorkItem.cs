using System;
using System.Threading;
using PoorManWorkManager;

namespace ConsoleApp
{
    public class SyncedDummyWorkItem : PoorManSyncedWorkItem
    {
        public int Id { get; }

        public SyncedDummyWorkItem(
            int id, PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken) 
            : base(workBatchSynchronizer, cancellationToken)
        {
            Id = id;
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