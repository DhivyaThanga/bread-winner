using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.PoorManWorkManager
{
    public class PoorManWorkManager<T> where T : IPoorManWorkItem, IDisposable
    {
        private readonly IPoorManWorker<T> _producer;
        private readonly IPoorManWorker<T> _consumerPool;
        private readonly BlockingCollection<T> _workQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public PoorManWorkManager(int concurrency, int workCheckingBackoff_ms, Func<T> workFactoryMethod)
        {
            _producer = new PoorManProducer<T>(workCheckingBackoff_ms, workFactoryMethod);
            _consumerPool = new PoorManConsumerPool<T>(concurrency);
            _workQueue = new BlockingCollection<T>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _producer.Start(_workQueue, _cancellationTokenSource.Token);
            _consumerPool.Start(_workQueue, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _producer.Stop();
            _consumerPool.Stop();

            _cancellationTokenSource.Dispose();
        }
    }
}