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

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            Container.Install(
                new ControllerInstaller(),
                new DependencyInstaller());

            config.UseWindsorContainer(Container);
            config.UseDefaultJsonConverter();
            config.UseDefaultRoutes();

            BoundedBufferStartup.Start(appBuilder);

            appBuilder
                .UseWindsorScopeMidddleware()
                .UseWebApi(config);
        }
    }
}