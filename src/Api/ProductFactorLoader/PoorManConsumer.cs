using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace Api.ProductFactorLoader
{
    public class PoorManConsumer : IPoorManConsumer
    {
        private volatile bool _isStarted = false;

        internal Thread WrappedThread { get; private set; }

        public void Start<T>(BlockingCollection<T> workQueue, CancellationToken cancellationToken) where T : IPoorManWorkItem
        {
            if (!_isStarted)
            {
                _isStarted = true;

                WrappedThread = new Thread(() => DoWork(workQueue, cancellationToken))
                {
                    IsBackground = true
                };

                WrappedThread.Start();
            }
            else
            {
                throw new ApplicationException($"{nameof(PoorManConsumer)} already started");
            }
        }

        private void DoWork<T>(BlockingCollection<T> workQueue, CancellationToken cancellationToken) 
            where T : IPoorManWorkItem
        {
            while (true)
            {
                var workItem = workQueue.Take(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
                workItem.Do();
            }
        }
    }
}