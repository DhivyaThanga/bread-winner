using System.Threading;
using Microsoft.Owin.BuilderProperties;
using Owin;
using PoorManWork;

namespace Api
{
    public static class AppBuilderExtensions
    {
        public static CancellationToken GetOnAppDisposing(
            this IAppBuilder appBuilder)
        {
            return new AppProperties(appBuilder.Properties).OnAppDisposing;
        }
    }
}