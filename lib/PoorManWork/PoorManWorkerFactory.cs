using System;
using System.Threading;
using PoorManWork;

namespace PoorManWorkManager
{
    public class PoorManWorkerFactory : IPoorManWorkerFactory
    {
        public IPoorManWorker CreateConsumer()
        {
            return new PoorManConsumer();
        }

        public IPoorManWorker CreateProducer(
            EventWaitHandle workArrived,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            return new PoorManProducer(workArrived, workFactoryMethod);
        }

        public IPoorManWorker[] CreateConsumerArray(int size)
        {
            var consumers = new IPoorManWorker[size];
            for (int i = 0; i < consumers.Length; i++)
            {
                consumers[i] = CreateConsumer();
            }

            return consumers;
        }

    }
}
