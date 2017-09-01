using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BreadWinner
{
    public class WorkerFactory : IWorkerFactory
    {
        private readonly BlockingCollection<IWorkItem> _workQueue;

        public WorkerFactory()
        {
            _workQueue = new BlockingCollection<IWorkItem>();
        }

        public IWorker CreateProducer(
            Func<AbstractProducer> factoryMethod)
        {
            var producer = factoryMethod();
            producer.AddWork = AddWork;

            return producer;
        }

        public IWorkerPool CreatePool()
        {
            return new WorkerPool();
        }

        public IWorker CreateScheduledJob(
            TimeSpan interval,
            Action<CancellationToken> workItem, 
            Action<CancellationToken> startupAction = null)
        {
            return new ScheduledJob(interval, workItem, startupAction);
        }

        public IWorker[] CreateConsumers(int n)
        {
            var consumers = new List<IWorker>();
            for (var i = 0; i < n; i++)
            {
                consumers.Add(new Consumer(TakeWork));
            }

            return consumers.ToArray();
        }

        private void AddWork(IWorkItem[] workBatch, CancellationToken cancellationToken)
        {
            foreach (var workItem in workBatch)
            {
                _workQueue.Add(workItem, cancellationToken);
            }
        }

        private IWorkItem TakeWork(CancellationToken cancellationToken)
        {
            return _workQueue.Take(cancellationToken);
        }
    }
}