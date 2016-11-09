using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Happer.Hosting.Self;

namespace Happer.TestHttpServer
{
    public class HybridSelfHost : SelfHost
    {
        private IHybridEngine _engine;

        public HybridSelfHost(IHybridEngine engine, params Uri[] baseUris)
            : base(engine, baseUris)
        {
            _engine = engine;
        }

        protected override async Task Process(HttpListenerContext httpContext)
        {
            try
            {
                var cancellationToken = new CancellationToken();

                // Each request is processed in its own execution thread.
                if (httpContext.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await httpContext.AcceptWebSocketAsync(null);
                    var baseUri = GetBaseUri(webSocketContext.RequestUri);
                    if (baseUri == null)
                        throw new InvalidOperationException(string.Format(
                            "Unable to locate base URI for request: {0}", webSocketContext.RequestUri));
                    await _engine.HandleWebSocket(httpContext, webSocketContext, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var baseUri = GetBaseUri(httpContext.Request.Url);
                    if (baseUri == null)
                        throw new InvalidOperationException(string.Format(
                            "Unable to locate base URI for request: {0}", httpContext.Request.Url));
                    var context = await _engine.HandleHttp(httpContext, baseUri, cancellationToken).ConfigureAwait(false);
                    context.Dispose();
                }
            }
            catch (NotSupportedException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                httpContext.Response.Close();
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                httpContext.Response.Close();
            }
        }
    }
}
