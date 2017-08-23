using System.Threading;

namespace PoorManWorkManager
{
    public abstract class PoorManSyncedWorkItem : IPoorManWorkItem
    {
        private readonly PoorManSynchronizer _synchronizer;
        private readonly CancellationToken _cancellationToken;

        internal PoorManSyncedWorkItem(PoorManSynchronizer synchronizer, CancellationToken cancellationToken)
        {
            _synchronizer = synchronizer;
            _cancellationToken = cancellationToken;
        }

        public void Do(CancellationToken cancellationToken)
        {
            DoAlways(_cancellationToken);
            if (_synchronizer.WorkDone())
            {
                DoFinally(_cancellationToken);
            }
        }

        protected abstract void DoAlways(CancellationToken cancellationToken);

        protected abstract void DoFinally(CancellationToken cancellationToken);
    }
}