using System;
using System.Threading;
using System.Threading.Tasks;

namespace PoorManWork
{
    public class PoorManPulser
    {
        private readonly Task _pulser;

        internal EventWaitHandle WorkArrived { get; }

        public PoorManPulser(TimeSpan interval, Action updateStateAction, CancellationToken cancellationToken)
        {
            WorkArrived = new AutoResetEvent(false);

            _pulser = new Task(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    updateStateAction();
                    WorkArrived.Set();
                    cancellationToken.WaitHandle.WaitOne(interval);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            _pulser.Start();
        }

        public void Stop()
        {
            _pulser.Wait();
        }
    }
}