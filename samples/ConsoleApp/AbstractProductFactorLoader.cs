using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public abstract class AbstractProductFactorLoader : IDisposable
    {
        private readonly IPoorManWorkFacade _poorManWorkFacade;
        protected static int Count;
        private readonly PoorManPulser _workEmitter;
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected AbstractProductFactorLoader()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _workEmitter = new PoorManPulser(
                new TimeSpan(days:0, hours:0, minutes:0, seconds:10), 
                () =>
                {
                    Console.WriteLine("Dummy Pulser: hearthbeat...");
                    Interlocked.Exchange(ref Count, 0);
                }, 
                _cancellationTokenSource.Token);

            _poorManWorkFacade = new PoorManWorkFacade(_cancellationTokenSource.Token);
            _poorManWorkFacade.AddConsumers(2);
            _poorManWorkFacade.AddProducer(_workEmitter, WorkBatchFactoryMethod);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _workEmitter.Stop();
        }

        public void Start()
        {
            _poorManWorkFacade.Start();
            _workEmitter.Start();
        }

        protected abstract IPoorManWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken);
    }
}