using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public abstract class PoorManWorker<T> : IPoorManWorker<T> where T : IPoorManWorkItem
    {
        protected Thread WrappedThread { get; set; }

        public void Start(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            WrappedThread = new Thread(GetThreadAction(workQueue, cancellationToken))
            {
                IsBackground = true
            };

            WrappedThread.Start();
        }

        private ThreadStart GetThreadAction(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            return () =>
            {
                try
                {
                    Loop(workQueue, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Cancellation was requested
                }
            };
        }

        protected abstract void Loop(BlockingCollection<T> workQueue, CancellationToken cancellationToken);

        public void Stop()
        {
            if (!WrappedThread.Join(1000))
            {
                WrappedThread.Abort();
            }
        }
    }
}