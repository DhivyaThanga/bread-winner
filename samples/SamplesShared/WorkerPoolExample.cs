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
            var factory = new WorkerFactory();
            var workerPool = factory.CreatePool();

            var workAvailableRepo = new WorkAvailableRepo(1);
            workerPool.Add(
                factory.CreateScheduledJob(
                    workArrivedSchedule, token => { workAvailableRepo.Reset(); }));

            var started = new ManualResetEvent(false);
            var workFactory = GetWorkFactory(dummy, workerPool, workAvailableRepo);
            workerPool.Add(factory.CreateProducer(
                () => new ScheduledProducer(
                    producerCheckSchedule,
                    workFactory.Create,
                    (token) => workFactory.Startup(token, started),
                    started)));

            workerPool.Add(factory.CreateConsumers(consumers));

            workerPool.Start(cancellationToken);
            started.WaitOne();
        }

        private static ISampleWorkFactory GetWorkFactory(bool dummy, IWorkerPool workerPool, WorkAvailableRepo workAvailableRepo)
        {
            if (!dummy)
            {
                return new ReadFromBlobSampleWorkFactory(
                    () => workerPool.IsAlive, workAvailableRepo);
            }

            return new DummyWorkFactory(workAvailableRepo);
        }
    }
}