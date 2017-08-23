using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManConsumer : PoorManWorker
    {
        protected override void Loop(
            BlockingCollection<IPoorManWorkItem> workQueue, CancellationToken cancellationToken) 
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = workQueue.Take(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                workItem.Do(cancellationToken);
            }
        }
    }
}