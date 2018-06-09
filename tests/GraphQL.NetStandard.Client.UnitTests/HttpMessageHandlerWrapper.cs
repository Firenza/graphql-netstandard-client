using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphQL.NetStandard.Client.UnitTests
{
    // Provides a means of a bridge between the hard to mock protected abstract SendRequest() method and an easy to mock virtual method Send()
    public class HttpMessageHandlerWrapper : HttpMessageHandler
    {
        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException("Now we can setup this method with our mocking framework");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
