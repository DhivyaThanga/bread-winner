using System;
using BreadWinner;
using SamplesShared.BlobExample;
using SamplesShared.DummyExample;

namespace SamplesShared
{
    public class WorkerPoolExample
    {

        public static IWorkerPool CreatePool(bool dummy,
            TimeSpan workArrivedSchedule, TimeSpan producerCheckSchedule, int consumers)
        {
            var factory = new WorkerFactory();
            var workerPool = factory.CreatePool();

            var workAvailableRepo = new WorkAvailableRepo(1);
            workerPool.Add(
                factory.CreateScheduledJob(
                    workArrivedSchedule, token => { workAvailableRepo.Reset(); }));

            var workFactory = GetWorkFactory(dummy, workerPool, workAvailableRepo);
            workerPool.Add(factory.CreateProducer(
                () => new ScheduledProducer(
                    producerCheckSchedule,
                    workFactory.Create,
                    workFactory.Startup)));

            workerPool.Add(factory.CreateConsumers(consumers));

            return workerPool;
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