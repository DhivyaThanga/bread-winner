using System.Web.Http;
using Api;
using Api.IoC;
using Api.IoC.Extensions;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;

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

            appBuilder
                .UseWindsorScopeMidddleware()
                .UseWebApi(config);
        }
    }
}