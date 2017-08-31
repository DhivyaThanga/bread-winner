using System;
using System.Threading;

namespace PoorManWork
{
    public class PoorManPulser : PoorManWorker
    {
        private readonly TimeSpan _interval;
        private readonly Action _updateStateAction;

        internal EventWaitHandle WorkArrived { get; }

        public PoorManPulser(TimeSpan interval,  Action updateStateAction)
        {
            _interval = interval;
            _updateStateAction = updateStateAction;
            WorkArrived = new AutoResetEvent(false);
        }

        protected override void Loop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _updateStateAction?.Invoke();
                WorkArrived.Set();
                cancellationToken.WaitHandle.WaitOne(_interval);
            }
        }
    }
}