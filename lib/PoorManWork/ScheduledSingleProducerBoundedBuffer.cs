using System;
using System.Threading;
using PoorManWorkManager;

namespace PoorManWork
{
    public class ScheduledSingleProducerBoundedBuffer : IPoorManWorker
    {
        private readonly IPoorManManager _poorManBoundedBuffer;
        protected static int Count;
        private readonly PoorManPulser _workEmitter;

        public ScheduledSingleProducerBoundedBuffer(
            PoorManPulser pulser,
            int consumers,
            Func<CancellationToken, IPoorManWorkItem[]> workFactoryMethod)
        {
            _workEmitter = pulser;
            _poorManBoundedBuffer = new PoorManBoundedBuffer();
            _poorManBoundedBuffer.AddConsumers(consumers);
            _poorManBoundedBuffer.AddProducer(_workEmitter, workFactoryMethod);
        }

        public ScheduledSingleProducerBoundedBuffer(
            PoorManPulser pulser,
            int consumers,
            IPoorManWorkFactory workFactory)
            : this(pulser, consumers, workFactory.Create)
        {
        }

        /// <summary>
        /// Calling this method will make the bounded buffer and the configured pulser start
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Start(CancellationToken cancellationToken)
        {
            _poorManBoundedBuffer.Start(cancellationToken);
            _workEmitter.Start();
        }
    }
}