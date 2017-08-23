using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWorkManager
{
    public class PoorManWorkFacade<T> : IPoorManWorkFacade where T : IPoorManWorkItem
    {
        private readonly IPoorManWorker<T> _producer;
        private readonly IPoorManWorker<T> _consumerPool;
        private readonly BlockingCollection<T> _workQueue;
        private volatile bool _isStarted = false;

        public PoorManWorkFacade(int concurrency,
            EventWaitHandle workArrived,
            Func<CancellationToken, T[]> workFactoryMethod)
        {
            _producer = new PoorManProducer<T>(workArrived, workFactoryMethod);
            var consumers = InstantiateConsumers(concurrency);
            _consumerPool = new PoorManWorkerPool<T>(consumers);
            _workQueue = new BlockingCollection<T>();
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (!_isStarted)
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

        private static IPoorManWorker<T>[] InstantiateConsumers(int concurrency)
        {
            var consumers = new IPoorManWorker<T>[concurrency];
            for (var i = 0; i < consumers.Length; i++)
            {
                consumers[i] = new PoorManConsumer<T>();
            }
            return consumers;
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