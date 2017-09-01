using System.Web.Http;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using WebApi;
using WebApi.Extensions;
using WebApi.IoC.Extensions;
using WebApi.IoC.Installer;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApi
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

            new BoundedBufferStartup().Start(appBuilder);

            appBuilder
                .UseWindsorScopeMidddleware()
                .UseWebApi(config);
        }
    }
}