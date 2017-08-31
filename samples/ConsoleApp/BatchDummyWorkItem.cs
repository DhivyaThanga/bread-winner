using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public class BatchDummyWorkItem : BatchWorkItem
    {
        public BatchDummyWorkItem(
            int id, WorkBatch workBatch, CancellationToken cancellationToken) 
            : base(id.ToString(), workBatch, cancellationToken)
        {
        }

        protected override void DoAlways(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");
            cancellationToken.WaitHandle.WaitOne(2000);
        }

        protected override void DoAlwaysErrorCallback(Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId}, consuming {Id}, LAST OF THE BATCH");
        }
    }
}