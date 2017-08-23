using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class PoorManConsumer : PoorManWorker
    {
        private readonly Func<CancellationToken, IPoorManWorkItem> _takeWork;

        internal PoorManConsumer(Func<CancellationToken, IPoorManWorkItem> takeWork)
        {
            _takeWork = takeWork;
        }

        protected override void Loop(CancellationToken cancellationToken) 
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = _takeWork(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                workItem.Do(cancellationToken);
            }
        }
    }
}