using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWorkManager
{
    public class PoorManWorkManager<T> : IPoorManWorkManager<T> where T : IPoorManWorkItem
    {
        private IPoorManWorker<T> _producer;
        private IPoorManWorker<T> _consumerPool;
        private BlockingCollection<T> _workQueue;
        private CancellationTokenSource _cancellationTokenSource;
        private volatile bool _isStarted = false;

        public void Start(int concurrency, EventWaitHandle workArrived, Func<CancellationToken, T[]> workFactoryMethod)
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _producer = new PoorManProducer<T>(workArrived, workFactoryMethod);
                var consumers = InstantiateConsumers(concurrency);
                _consumerPool = new PoorManWorkerPool<T>(consumers);
                _workQueue = new BlockingCollection<T>();
                _cancellationTokenSource = new CancellationTokenSource();

                _consumerPool.Start(_workQueue, _cancellationTokenSource.Token);
                _producer.Start(_workQueue, _cancellationTokenSource.Token);
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

        public void Dispose()
        {
            if (!_isStarted) return;

            _isStarted = false;
            _cancellationTokenSource.Cancel();
            _producer.Stop();
            _consumerPool.Stop();

            _cancellationTokenSource.Dispose();
        }
    }
}