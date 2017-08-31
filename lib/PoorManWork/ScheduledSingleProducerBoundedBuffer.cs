using System.Threading;

namespace PoorManWork
{
    public class ScheduledSingleProducerBoundedBuffer : IPoorManWorker
    {
        private readonly IPoorManManager _poorManBoundedBuffer;
        protected static int Count;
        private readonly PoorManPulser _workEmitter;

        public bool IsAlive => _poorManBoundedBuffer.IsAlive && _workEmitter.IsAlive;

        public ScheduledSingleProducerBoundedBuffer(
            PoorManPulser pulser,
            int consumers,
            PoorManAbstractProducer producer)
        {
            _workEmitter = pulser;
            _poorManBoundedBuffer = new PoorManBoundedBuffer();
            _poorManBoundedBuffer.AddConsumers(consumers);

            _poorManBoundedBuffer.AddProducer(producer);
        }

        /// <summary>
        /// Calling this method will make the bounded buffer and the configured pulser start
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Start(CancellationToken cancellationToken)
        {
            _poorManBoundedBuffer.Start(cancellationToken);
            _workEmitter.Start(cancellationToken);
        }
    }
}