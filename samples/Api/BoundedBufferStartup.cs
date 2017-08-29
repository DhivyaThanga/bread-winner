using System;
using System.Diagnostics;
using Api.BlobExample;
using Owin;
using PoorManWork;

namespace Api
{
    public class BoundedBufferStartup
    {
        private ScheduledSingleProducerBoundedBuffer _boundedBuffer;

        public void Start(IAppBuilder appBuilder)
        {
            var cancellationToken = appBuilder.GetOnAppDisposing();

            var workAvailableRepo = new WorkAvailableRepo(new TimeSpan(0, 0, 10, 0), 1);
            var workFactory = new ReadFromBlobWorkFactory(() => _boundedBuffer != null && _boundedBuffer.IsAlive, workAvailableRepo);

            var pulser = new PoorManPulser(new TimeSpan(0, 0, 0, 10),
                () => { Debug.WriteLine("Dummy Pulser: hearthbeat..."); });

            _boundedBuffer = new ScheduledSingleProducerBoundedBuffer(pulser, 100, workFactory.Create);
            _boundedBuffer.Start(cancellationToken);
            workAvailableRepo.Start(appBuilder.GetOnAppDisposing());
        }
    }
}