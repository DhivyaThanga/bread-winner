using System;
using System.Runtime;
using System.Threading;
using BreadWinner;
using SamplesShared.BlobExample;
using SamplesShared.DummyExample;

namespace SamplesShared
{
    public class WorkerPoolExample
    {

        public static void StartPool(
            bool dummy,
            TimeSpan workArrivedSchedule, 
            TimeSpan producerCheckSchedule, 
            int consumers,
            CancellationToken cancellationToken)
        {
            var workAvailableRepo = new WorkAvailableRepo(6);
            var started = new ManualResetEvent(false);

            var workFactory = GetWorkFactory(dummy, workAvailableRepo);

            var workerPool =
                new WorkPoolBuilder()
                    .WithScheduledJob(
                        workArrivedSchedule, token => { workAvailableRepo.Reset(); })
                    .WithProducer(
                        ProducerFactoryMethod(producerCheckSchedule, workFactory, started))
                    .WithNConsumers(consumers)
                    .Build();

            workerPool.Start(cancellationToken);

            started.WaitOne();
        }

        private static Func<AbstractProducer> ProducerFactoryMethod(TimeSpan producerCheckSchedule, ISampleWorkFactory workFactory, ManualResetEvent started)
        {
            return () => new ScheduledProducer(
                producerCheckSchedule,
                workFactory.Create,
                (token) => workFactory.Startup(token, started),
                started);
        }

        private static ISampleWorkFactory GetWorkFactory(bool dummy, WorkAvailableRepo workAvailableRepo)
        {
            if (!dummy)
            {
                return new ReadFromBlobSampleWorkFactory(workAvailableRepo);
            }

            return new DummyWorkFactory(workAvailableRepo);
        }
    }
}