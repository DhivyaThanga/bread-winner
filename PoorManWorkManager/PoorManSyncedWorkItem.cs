using System.Threading;

namespace PoorManWorkManager
{
    public abstract class PoorManSyncedWorkItem : IPoorManWorkItem
    {
        private readonly PoorManWorkBatchSynchronizer _workBatchSynchronizer;
        private readonly CancellationToken _cancellationToken;

        protected PoorManSyncedWorkItem(PoorManWorkBatchSynchronizer workBatchSynchronizer, CancellationToken cancellationToken)
        {
            _workBatchSynchronizer = workBatchSynchronizer;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            DoAlways(_cancellationToken);
            if (_workBatchSynchronizer.WorkDone())
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}