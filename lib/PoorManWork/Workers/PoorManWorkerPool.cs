using System;
using System.Collections.Generic;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManWorkerPool : IPoorManWorker
    {
        private readonly List<IPoorManWorker> _workers;
        private volatile bool _isStarted = false;

        public PoorManWorkerPool()
        {
            _workers = new List<IPoorManWorker>();
        }

        public void Add(params IPoorManWorker[] workers)
        {
            _workers.AddRange(workers);
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (!_isStarted && !cancellationToken.IsCancellationRequested)
            {
                _isStarted = true;
                foreach (var worker in _workers)
                {
                    worker.Start(cancellationToken);
                }
            }
            else
            {
                throw new ApplicationException("Workers already started");
            }
        }
    }
}