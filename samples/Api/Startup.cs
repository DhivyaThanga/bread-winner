using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Http;
using Api;
using Api.Extensions;
using Api.IoC;
using Api.IoC.Extensions;
using Castle.Windsor;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Owin;
using PoorManWork;

[assembly: OwinStartup(typeof(Startup))]

namespace Api
{
    public class Startup
    {
        protected static readonly IWindsorContainer Container = new WindsorContainer();
        private int _count;

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            Container.Install(
                new ControllerInstaller(),
                new DependencyInstaller());

            config.UseWindsorContainer(Container);
            config.UseDefaultJsonConverter();
            config.UseDefaultRoutes();

            RegisterBoundedBuffer(appBuilder);

            appBuilder
                .UseWindsorScopeMidddleware()
                .UseWebApi(config);
        }

        private void RegisterBoundedBuffer(IAppBuilder appBuilder)
        {
            var cancellationToken = new AppProperties(appBuilder.Properties).OnAppDisposing;
            var pulser = new PoorManPulser(new TimeSpan(0, 0, 0, 10), cancellationToken, () =>
            {
                Debug.WriteLine("Dummy Pulser: hearthbeat...");
                Interlocked.Exchange(ref _count, 0);
            });

            var boundedBuffer = new PoorManBoundedBuffer();
            boundedBuffer.AddProducer(pulser, WorkFactoryMethod);
            boundedBuffer.AddConsumers(2);
            boundedBuffer.Start(cancellationToken);
            pulser.Start();
        }

        private IPoorManWorkItem[] WorkFactoryMethod(CancellationToken cancellationToken)
        {
            if (_count > 1)
            {
                return null;
            }

            Interlocked.Increment(ref _count);

            if (cancellationToken.WaitHandle.WaitOne(1000))
            {
                return null;
            }

            var rand = new Random();
            var synchronizer = new PoorManWorkBatchSynchronizer(3);
            var workItems = new[]
            {
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken),
                new SyncedDummyWorkItem(rand.Next(), synchronizer, cancellationToken)
            };

            Debug.WriteLine(
                $"Producer {Thread.CurrentThread.ManagedThreadId} has created " +
                $"{workItems[0].Id}, {workItems[1].Id}, {workItems[2].Id}");

            return workItems.Cast<IPoorManWorkItem>().ToArray();
        }
    }
}