using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWorkManager
{
    public class PoorManWorkerPool<T> : IPoorManWorker<T> where T : IPoorManWorkItem
    {
        private readonly IPoorManWorker<T>[] _workers;
        private volatile bool _isStarted = false;

        public PoorManWorkerPool(IPoorManWorker<T>[] workers)
        {
            _workers = workers;
        }

        public void Start(BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            if (!_isStarted && !cancellationToken.IsCancellationRequested)
            {
                _isStarted = true;
                foreach (var consumer in _workers)
                {
                    consumer.Start(workQueue, cancellationToken);
                }
            }
            else
            {
                throw new ApplicationException("Workers already started");
            }
        }

        public void Stop()
        {
            if (!_isStarted) return;

            foreach (var consumer in _workers)
            {
                consumer.Stop();
            }

            _isStarted = false;
        }
    }
}