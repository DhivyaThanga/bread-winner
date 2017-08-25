using System;
using System.Threading;
using PoorManWork;

namespace ConsoleApp
{
    public class DummyWorkItem : IPoorManWorkItem
    {
        public string Id { get; }

        public PoorManWorkItemStatus WorkItemStatus { get; private set; }


        public DummyWorkItem(int id)
        {
            Id = id.ToString();
            WorkItemStatus = PoorManWorkItemStatus.Scheduled;
        }

        public void Do(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consuming {Id}");
                cancellationToken.WaitHandle.WaitOne(2000);
                WorkItemStatus = PoorManWorkItemStatus.Completed;
            }
            catch (Exception)
            {
                WorkItemStatus = PoorManWorkItemStatus.Failed;
            }
        }
    }
}