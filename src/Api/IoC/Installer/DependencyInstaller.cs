using Api.Dependency;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Api.IoC
{
    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IHttpClient>()
                .ImplementedBy<HttpClientStub>()
                .LifestyleSingleton());
        }
    }
}