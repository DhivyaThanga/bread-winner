using System;
using System.Threading;

namespace BreadWinner
{
    public abstract class AbstractBatchedWorkItem : IWorkItem
    {
        private readonly CancellationToken _cancellationToken;

        protected WorkBatch Batch { get; }
        public string Id { get; }
        public WorkItemStatus WorkItemStatus { get; protected set; }
        public object Result { get; protected set; }

        protected AbstractBatchedWorkItem(
            string id, WorkBatch batch, CancellationToken cancellationToken)
        {
            Id = id;
            Batch = batch;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            DoAlways(_cancellationToken);

            if (Batch.WorkDone(this))
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}