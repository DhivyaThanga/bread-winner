using System;
using System.Threading;
using System.Threading.Tasks;

namespace PoorManWork
{
    public class PoorManPulser
    {
        private readonly Task _pulser;

        internal EventWaitHandle WorkArrived { get; }

        public PoorManPulser(TimeSpan interval, CancellationToken cancellationToken, Action updateStateAction = null)
        {
            WorkArrived = new AutoResetEvent(false);

            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(Stop);
            }

            _pulser = new Task(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    updateStateAction?.Invoke();
                    WorkArrived.Set();
                    cancellationToken.WaitHandle.WaitOne(interval);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            _pulser.Start();
        }

        private void Stop()
        {
            if (_pulser.Status != TaskStatus.Created)
            {
                _pulser.Wait();
            }
        }
    }
}