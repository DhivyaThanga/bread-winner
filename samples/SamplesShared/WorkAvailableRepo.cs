using System.Threading;

namespace SamplesShared
{
    public class WorkAvailableRepo
    {
        private int _count;
        public int ConsecutiveAvailableBatches { get; }

        public WorkAvailableRepo(int consecutiveAvailableBatches)
        {
            ConsecutiveAvailableBatches = consecutiveAvailableBatches;
        }

        public void Reset()
        {
            CloudConsole.WriteLine("Work Arrived!!");
            Interlocked.Exchange(ref _count, -1);
        }

        public int WorkAvailableId()
        {
            Interlocked.Increment(ref _count);

            if (_count >= ConsecutiveAvailableBatches)
            {
                return -1;
            }

            return _count;
        }
    }
}