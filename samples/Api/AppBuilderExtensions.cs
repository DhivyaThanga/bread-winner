using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Castle.Windsor;
using Microsoft.Owin.BuilderProperties;
using Owin;
using PoorManWork;
using PoorManWorkManager;

namespace Api
{
    public static class AppBuilderExtensions
    {
        public static CancellationToken GetOnAppDisposing(
            this IAppBuilder appBuilder)
        {
            return new AppProperties(appBuilder.Properties).OnAppDisposing;
        }

        public static void StartBoundedBuffer(
            this IAppBuilder appBuilder,
            PoorManPulser pulser,
            int consumers,
            IPoorManWorkFactory workFactory)
        {
            var cancellationToken = appBuilder.GetOnAppDisposing();
            var boundedBuffer = new ScheduledSingleProducerBoundedBuffer(pulser, 2, workFactory.Create);
            boundedBuffer.Start(cancellationToken);
        }
    }
}