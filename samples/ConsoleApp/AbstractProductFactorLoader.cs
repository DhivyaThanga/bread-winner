using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public abstract class AbstractProductFactorLoader : IDisposable
    {
        private readonly PoorManWorkerPool _workerPool;
        protected static int Count = 2;
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected AbstractProductFactorLoader()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var factory = new WorkerFactory();
            _workerPool = new PoorManWorkerPool();

            _workerPool.Add(
                factory.CreateScheduledJob(
                new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 15),
                cancellationToken =>
                {
                    Console.WriteLine("Work arrived!");
                    Interlocked.Exchange(ref Count, 0);
                }));

            _workerPool.Add(factory.CreateProducer(
                () => new ScheduledProducer(
                    new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 10), 
                    WorkBatchFactoryMethod)));

            _workerPool.Add(factory.CreateConsumers(2));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            _workerPool.Start(_cancellationTokenSource.Token);
        }

        protected abstract IPoorManWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken);
    }
}