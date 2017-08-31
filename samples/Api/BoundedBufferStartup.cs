using System;
using System.Diagnostics;
using Api.BlobExample;
using Owin;
using PoorManWork;

namespace Api
{
    public class BoundedBufferStartup
    {
        private PoorManWorkerPool _workerPool;

        public void Start(IAppBuilder appBuilder)
        {
            var cancellationToken = appBuilder.GetOnAppDisposing();

            var workAvailableRepo = new WorkAvailableRepo(new TimeSpan(0, 0, 10, 0), 1);
            var workFactory = new ReadFromBlobWorkFactory(() => _workerPool != null && _workerPool.IsAlive, workAvailableRepo);

            var pulser = new PoorManPulser(new TimeSpan(0, 0, 0, 10),
                () => { Debug.WriteLine("Dummy Pulser: hearthbeat..."); });

            var factory = new WorkerFactory();
            _workerPool = new PoorManWorkerPool();
            _workerPool.Add(factory.CreateProducer(producerFactoryMethod));
            _workerPool.Add(factory.CreateConsumers(2));

            _workerPool.Start(cancellationToken);
            workAvailableRepo.Start(appBuilder.GetOnAppDisposing());
        }
    }
}