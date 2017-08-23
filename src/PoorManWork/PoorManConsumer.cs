using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    public class PoorManConsumer<T> : PoorManWorker<T> where T : IPoorManWorkItem
    {
        protected override void Loop(BlockingCollection<T> workQueue, CancellationToken cancellationToken) 
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