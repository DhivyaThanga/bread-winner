using System;
using BreadWinner;
using Owin;
using WebApi.BlobExample;

namespace WebApi
{
    public class BoundedBufferStartup
    {
        private IWorkerPool _workerPool;

        public void Start(IAppBuilder appBuilder)
        {
            var cancellationToken = appBuilder.GetOnAppDisposing();

            var factory = new WorkerFactory();
            _workerPool = factory.CreatePool();
            
            var workAvailableRepo = new WorkAvailableRepo(1);
            _workerPool.Add(
                factory.CreateScheduledJob(
                    new TimeSpan(0, 0, 10, 0), token => { workAvailableRepo.Reset(); }));

            var workFactory = new ReadFromBlobWorkFactory(
                () => _workerPool != null && _workerPool.IsAlive, workAvailableRepo);
            _workerPool.Add(factory.CreateProducer(
                () => new ScheduledProducer(
                    new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 10),
                    workFactory.Create)));

            _workerPool.Add(factory.CreateConsumers(2));

            _workerPool.Start(cancellationToken);
        }
    }
}