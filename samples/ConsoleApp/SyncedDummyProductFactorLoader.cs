﻿using System;
using System.Linq;
using System.Threading;
using BreadWinner;

namespace ConsoleApp
{
    public class SyncedDummyProductFactorLoader : AbstractProductFactorLoader
    {
        protected override IWorkItem[] WorkBatchFactoryMethod(CancellationToken cancellationToken)
        {
            if (Count > 1)
            {
                return null;
            }

            Interlocked.Increment(ref Count);

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var synchronizer = new WorkBatch(3);
            var workItems = new [] {
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new BatchDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };
            Console.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IWorkItem>().ToArray();
        }
    }
}