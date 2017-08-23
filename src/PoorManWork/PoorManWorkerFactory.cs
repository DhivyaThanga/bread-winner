using System;
using System.Threading;
using PoorManWork;

namespace PoorManWorkManager
{
    public class PoorManWorkerFactory<T> : IPoorManWorkerFactory<T> where T : IPoorManWorkItem
    {
        public IPoorManWorker<T> CreateConsumer()
        {
            return new PoorManConsumer<T>();
        }

        public IPoorManWorker<T> CreateProducer(
            EventWaitHandle workArrived,
            Func<CancellationToken, T[]> workFactoryMethod)
        {
            return new PoorManProducer<T>(workArrived, workFactoryMethod);
        }

        public IPoorManWorker<T>[] CreateConsumerArray(int size)
        {
            var consumers = new IPoorManWorker<T>[size];
            for (int i = 0; i < consumers.Length; i++)
            {
                consumers[i] = CreateConsumer();
            }

            return consumers;
        }

    }
}
