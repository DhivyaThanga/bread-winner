using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Dependency
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}