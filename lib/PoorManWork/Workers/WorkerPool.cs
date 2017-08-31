using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PoorManWork
{
    public class WorkerPool : IWorkerPool
    {
        private readonly List<IWorker> _workers;
        private volatile bool _isStarted = false;

        public bool IsAlive {
            get { return _workers.Select(x => !x.IsAlive).Any(); }
        }

        internal WorkerPool()
        {
            _workers = new List<IWorker>();
        }

        public void Add(params IWorker[] workers)
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