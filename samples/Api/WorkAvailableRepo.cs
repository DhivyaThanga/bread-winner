using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Api
{
    public class WorkAvailableRepo
    {
        private int _count;
        private readonly TimeSpan _schedule;
        private readonly int _consecutiveAvailableBatches;
        private Task _pulser;

        public WorkAvailableRepo(TimeSpan schedule, int consecutiveAvailableBatches)
        {
            _schedule = schedule;
            _consecutiveAvailableBatches = consecutiveAvailableBatches;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _pulser = new Task(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Reset();
                    cancellationToken.WaitHandle.WaitOne(_schedule);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning);

            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(_pulser.Wait);
            }

            _pulser.Start();
        }

        public void Reset()
        {
            Debug.WriteLine("Work Arrived!!");
            Interlocked.Exchange(ref _count, 0);
        }

        public bool IsWorkAvailable()
        {
            if (_count >= _consecutiveAvailableBatches)
            {
                return false;
            }

            Interlocked.Increment(ref _count);

            return true;
        }
    }
}