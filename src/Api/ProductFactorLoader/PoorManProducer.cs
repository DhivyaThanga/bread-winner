using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Api.ProductFactorLoader
{
    public class PoorManProducer
    {
        internal Thread WrappedThread { get; private set; }

        private volatile bool _isStarted = false;

        public void Start<T>(int everyNms, Func<T> workFactoryMethod, BlockingCollection<T> workQueue, CancellationToken cancellationToken)
            where T : IPoorManWorkItem
        {
            if (!_isStarted)
            {
                _isStarted = true;

                WrappedThread = new Thread(() => PushWork(everyNms, workFactoryMethod, workQueue, cancellationToken))
                {
                    IsBackground = true
                };
            }
            else
            {
                throw new ApplicationException($"{nameof(PoorManProducer)} already started");
            }
        }

        private void PushWork<T>(int everyNms, Func<T> workFactoryMethod, BlockingCollection<T> workQueue, CancellationToken cancellationToken)
        {
            while (true)
            {
                GetAllAvailableWork(workFactoryMethod, workQueue, cancellationToken);

                if (cancellationToken.WaitHandle.WaitOne(everyNms)) break;
            }
        }

        private static void GetAllAvailableWork<T>(Func<T> workFactoryMethod, BlockingCollection<T> workQueue,
            CancellationToken cancellationToken)
        {
            bool workToDo = true;

            while (workToDo)
            {
                var workItem = workFactoryMethod();
                if (workItem != null)
                {
                    workQueue.Add(workItem, cancellationToken);
                }
                else
                {
                    workToDo = false;
                }
            }
        }
    }
}