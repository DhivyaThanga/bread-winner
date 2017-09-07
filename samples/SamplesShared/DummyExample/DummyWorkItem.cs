using System;
using System.Diagnostics;
using System.Threading;
using BreadWinner;
using SamplesShared.BlobExample;

namespace SamplesShared.DummyExample
{
    public class DummyWorkItem : AbstractBatchedWorkItem
    {
        public DummyWorkItem(
            int id, WorkBatch batch, CancellationToken cancellationToken) 
            : base(id.ToString(), batch, cancellationToken)
        {
        }

        protected override WorkStatus DoAlways(CancellationToken cancellationToken)
        {
            CloudConsole.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");

            cancellationToken.WaitHandle.WaitOne(2000);

            return WorkStatus.Successful;
        }

        protected override void DoFinally(CancellationToken cancellationToken)
        {
            CloudConsole.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}, LAST OF THE BATCH");
        }
    }
}