using System.Diagnostics;
using System.Threading;
using BreadWinner;
using SamplesShared.BlobExample;

namespace SamplesShared
{
    public class WorkAvailableRepo
    {
        private int _count;
        private readonly int _consecutiveAvailableBatches;
        public IWorker Job { get; }


        public WorkAvailableRepo(int consecutiveAvailableBatches)
        {
            _consecutiveAvailableBatches = consecutiveAvailableBatches;
        }

        public void Reset()
        {
            CloudConsole.WriteLine("Work Arrived!!");
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