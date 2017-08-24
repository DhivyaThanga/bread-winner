using System;
using System.Threading;
using PoorManWork;

namespace Api
{
    public class ScheduledSingleProducerBoundedBuffer : IDisposable
    {
        private readonly IPoorManManager _poorManBoundedBuffer;
        protected static int Count;
        private readonly PoorManPulser _workEmitter;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ScheduledSingleProducerBoundedBuffer(
            TimeSpan schedule,
            int consumers, 
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _workEmitter = new PoorManPulser(schedule, _cancellationTokenSource.Token);

            _poorManBoundedBuffer = new PoorManBoundedBuffer();
            _poorManBoundedBuffer.AddConsumers(consumers);
            _poorManBoundedBuffer.AddProducer(_workEmitter, workFactoryMethod);
        }

        public ScheduledSingleProducerBoundedBuffer(
            PoorManPulser pulser,
            CancellationToken cancellationToken,
            int consumers,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _workEmitter = pulser;
            _cancellationToken = cancellationToken;

            _poorManBoundedBuffer = new PoorManBoundedBuffer();
            _poorManBoundedBuffer.AddConsumers(consumers);
            _poorManBoundedBuffer.AddProducer(_workEmitter, workFactoryMethod);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Start()
        {
            var cancellationToken = _cancellationTokenSource?.Token ?? _cancellationToken;
            _poorManBoundedBuffer.Start(cancellationToken);
            _workEmitter.Start();
        }
    }
}