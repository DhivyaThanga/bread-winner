using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoorManWork
{
    internal class Consumer : Worker
    {
        private readonly Func<CancellationToken, IWorkItem> _takeWork;

        internal Consumer(Func<CancellationToken, IWorkItem> takeWork)
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