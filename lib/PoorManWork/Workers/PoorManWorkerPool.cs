using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkerPool : IPoorManWorker
    {
        private readonly List<IPoorManWorker> _workers;
        private volatile bool _isStarted = false;

        public bool IsAlive {
            get { return _workers.Select(x => !x.IsAlive).Any(); }
        }

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