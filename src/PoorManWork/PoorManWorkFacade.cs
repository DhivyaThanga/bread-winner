using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManWorkFacade<T> : IPoorManWorkFacade where T : IPoorManWorkItem
    {
        private readonly IPoorManWorker<T> _producer;
        private readonly IPoorManWorker<T> _consumerPool;
        private readonly BlockingCollection<T> _workQueue;
        private volatile bool _isStarted = false;

        public PoorManWorkFacade(IPoorManWorker<T> producer, IPoorManWorker<T>[] consumers)
        {
            _producer = producer;
            _consumerPool = new PoorManWorkerPool<T>(consumers);
            _workQueue = new BlockingCollection<T>();
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
                    $"{nameof(PoorManWorkFacade<T>)} not started, cannot stop");
            }

            _isStarted = false;
            _producer.Stop();
            _consumerPool.Stop();
        }
    }
}