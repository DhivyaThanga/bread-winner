using System;
using System.Threading;

namespace PoorManWork
{
    internal class ScheduledJob : PoorManWorker
    {
        private readonly TimeSpan _interval;
        private readonly Action<CancellationToken> _workItem;

        internal ScheduledJob(
            TimeSpan interval, 
            Action<CancellationToken> workItem, 
            Action<CancellationToken> startupAction)
        {
            _interval = interval;
            _workItem = workItem;
            StartupAction = startupAction;
        }

        protected override void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.WaitHandle.WaitOne(_interval);
                _workItem?.Invoke(cancellationToken);
            }
        }
    }
}