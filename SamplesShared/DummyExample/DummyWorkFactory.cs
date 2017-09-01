using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BreadWinner;

namespace SamplesShared.DummyExample
{
    public class DummyWorkFactory
    {
        private readonly WorkAvailableRepo _workAvailableRepo;

        public DummyWorkFactory(WorkAvailableRepo workAvailableRepo)
        {
            _workAvailableRepo = workAvailableRepo;
        }

        public IWorkItem[] Create(CancellationToken cancellationToken)
        {
            if (!_workAvailableRepo.IsWorkAvailable())
            {
                return null;
            }

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var synchronizer = new WorkBatch(3);
            var workItems = new[]
            {
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };

            Debug.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IWorkItem>().ToArray();
        }
    }
}