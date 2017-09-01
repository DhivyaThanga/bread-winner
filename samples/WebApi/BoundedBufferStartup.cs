using System;
using System.Threading;
using BreadWinner;
using Owin;
using SamplesShared;
using SamplesShared.BlobExample;

namespace WebApi
{
    public class BoundedBufferStartup
    {
        private IWorkerPool _workerPool;

        public BoundedBufferStartup()
        {
        }

        public void Start(IAppBuilder appBuilder)
        {
            var pool = WorkerPoolExample.CreatePool(
                new TimeSpan(0, 0, 0, 15), 
                new TimeSpan(0, 0, 0, 10),
                2);

            pool.Start(appBuilder.GetOnAppDisposing());
        }
    }
}