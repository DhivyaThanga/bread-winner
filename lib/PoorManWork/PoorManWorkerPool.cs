using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManWorkerPool : IPoorManWorker
    {
        private readonly IPoorManWorker[] _workers;
        private volatile bool _isStarted = false;

        public PoorManWorkerPool(IPoorManWorker[] workers)
        {
            _workers = workers;
        }

        public void Start(BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken)
        {
            if (!_isStarted && !cancellationToken.IsCancellationRequested)
            {
                _isStarted = true;
                foreach (var worker in _workers)
                {
                    worker.Start(workQueue, cancellationToken);
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