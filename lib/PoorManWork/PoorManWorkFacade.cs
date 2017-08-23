using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkFacade : IPoorManWorkFacade
    {
        private readonly IPoorManWorker _producer;
        private readonly IPoorManWorker _consumerPool;
        private readonly BlockingCollection<IPoorManWorkItem> _workQueue;
        private volatile bool _isStarted = false;

        public PoorManWorkFacade(IPoorManWorker producer, IPoorManWorker[] consumers)
        {
            _producer = producer;
            _consumerPool = new PoorManWorkerPool(consumers);
            _workQueue = new BlockingCollection<IPoorManWorkItem>();
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (!_isStarted && !cancellationToken.IsCancellationRequested)
            {
                _isStarted = true;

                _consumerPool.Start(_workQueue, cancellationToken);
                _producer.Start(_workQueue, cancellationToken);
            }
            else
            {
                throw new ApplicationException("Manager already started");
            }
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                throw new ApplicationException(
                    $"{nameof(PoorManWorkFacade)} not started, cannot stop");
            }

            _isStarted = false;
            _producer.Stop();
            _consumerPool.Stop();
        }
    }
}