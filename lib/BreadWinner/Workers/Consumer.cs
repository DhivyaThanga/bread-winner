using System;
using System.Threading;

namespace BreadWinner
{
    public class Consumer : Worker
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