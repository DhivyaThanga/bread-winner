using System;
using System.Diagnostics;
using Owin;
using PoorManWork;

namespace Api
{
    public class BoundedBufferStartup
    {
        public static void Start(IAppBuilder appBuilder)
        {
            var cancellationToken = appBuilder.GetOnAppDisposing();

            var workAvailableRepo = new WorkAvailableRepo(new TimeSpan(0, 0, 0, 15), 2);
            var workFactory = new DummyWorkFactory(workAvailableRepo);

            var pulser = new PoorManPulser(new TimeSpan(0, 0, 0, 10), cancellationToken,
                () => { Debug.WriteLine("Dummy Pulser: hearthbeat..."); });

            var boundedBuffer = new ScheduledSingleProducerBoundedBuffer(pulser, 100, workFactory.Create);
            boundedBuffer.Start(cancellationToken);
            workAvailableRepo.Start(appBuilder.GetOnAppDisposing());
        }
    }
}