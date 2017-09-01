using System;
using BreadWinner;
using SamplesShared.BlobExample;
using SamplesShared.DummyExample;

namespace SamplesShared
{
    public class WorkerPoolExample
    {

        public static IWorkerPool CreatePool(
            TimeSpan workArrivedSchedule, TimeSpan producerCheckSchedule, int consumers)
        {
            var factory = new WorkerFactory();
            var workerPool = factory.CreatePool();

            var workAvailableRepo = new WorkAvailableRepo(2);
            workerPool.Add(
                factory.CreateScheduledJob(
                    workArrivedSchedule, token => { workAvailableRepo.Reset(); }));

            var workFactory = new DummyWorkFactory(workAvailableRepo);
            workerPool.Add(factory.CreateProducer(
                () => new ScheduledProducer(
                    producerCheckSchedule,
                    workFactory.Create,
                    workFactory.Startup)));

            workerPool.Add(factory.CreateConsumers(consumers));

            return workerPool;
        }
    }
}