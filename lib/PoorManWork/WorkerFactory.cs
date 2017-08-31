using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PoorManWork
{
    public class WorkerFactory : IWorkerFactory
    {
        private readonly BlockingCollection<IPoorManWorkItem> _workQueue;

        public WorkerFactory()
        {
            _workQueue = new BlockingCollection<IPoorManWorkItem>();
        }

        public IPoorManWorker CreateProducer(
            Func<PoorManAbstractProducer> factoryMethod)
        {
            var producer = factoryMethod();
            producer.AddWork = AddWork;

            return producer;
        }

        public IPoorManWorker CreateScheduledJob(
            TimeSpan interval,
            Action<CancellationToken> workItem, 
            Action<CancellationToken> startupAction = null)
        {
            return new ScheduledJob(interval, workItem, startupAction);
        }

        public IPoorManWorker[] CreateConsumers(int n)
        {
            var consumers = new List<IPoorManWorker>();
            for (var i = 0; i < n; i++)
            {
                consumers.Add(new PoorManConsumer(TakeWork));
            }

            return consumers.ToArray();
        }

        private void AddWork(IPoorManWorkItem[] workBatch, CancellationToken cancellationToken)
        {
            foreach (var workItem in workBatch)
            {
                _workQueue.Add(workItem, cancellationToken);
            }
        }

        private IPoorManWorkItem TakeWork(CancellationToken cancellationToken)
        {
            return _workQueue.Take(cancellationToken);
        }
    }
}