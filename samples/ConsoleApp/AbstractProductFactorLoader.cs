using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public abstract class AbstractProductFactorLoader : IDisposable
    {
        private readonly IPoorManManager _poorManBoundedBuffer;
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
                });

            _poorManBoundedBuffer = new PoorManBoundedBuffer();
            _poorManBoundedBuffer.AddConsumers(2);
            _poorManBoundedBuffer.AddProducer(_workEmitter, WorkBatchFactoryMethod);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            _poorManBoundedBuffer.Start(_cancellationTokenSource.Token);
            _workEmitter.Start(_cancellationTokenSource.Token);
        }

        protected abstract IPoorManWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken);
    }
}