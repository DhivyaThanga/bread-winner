using System;
using System.Threading;

namespace BreadWinner
{
    public abstract class AbstractBatchedWorkItem : IWorkItem
    {
        private readonly CancellationToken _cancellationToken;
        private int _status;

        protected IWorkBatch Batch { get; }
        public string Id { get; }
        public object Result { get; protected set; }
        public WorkStatus Status
        {
            get { return (WorkStatus)_status; }
            set { Interlocked.Exchange(ref _status, (int)value); }
        }

        protected AbstractBatchedWorkItem(
            string id, IWorkBatch batch, CancellationToken cancellationToken)
        {
            Id = id;
            Batch = batch;
            _cancellationToken = cancellationToken;
            Status = WorkStatus.Scheduled;
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