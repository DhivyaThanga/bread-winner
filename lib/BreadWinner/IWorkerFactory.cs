using System;
using System.Threading;

namespace BreadWinner
{
    public interface IWorkerFactory
    {
        IWorkerPool CreatePool();

        IWorker CreateScheduledJob(
            TimeSpan interval, 
            Action<CancellationToken> workItem,
            Action<CancellationToken> startupAction = null);

        IWorker[] CreateConsumers(int n);

        IWorker CreateProducer(Func<AbstractProducer> factoryMethod);
    }
}