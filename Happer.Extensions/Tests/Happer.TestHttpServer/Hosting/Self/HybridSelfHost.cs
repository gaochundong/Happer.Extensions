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

        protected override async Task Process(HttpListenerContext listenerContext, CancellationToken cancellationToken)
        {
            try
            {
                // Each request is processed in its own execution thread.
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await listenerContext.AcceptWebSocketAsync(null);
                    var baseUri = GetBaseUri(webSocketContext.RequestUri);
                    if (baseUri == null)
                        throw new InvalidOperationException(string.Format(
                            "Unable to locate base URI for request: {0}", webSocketContext.RequestUri));
                    await _engine.HandleWebSocket(listenerContext, webSocketContext, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var baseUri = GetBaseUri(listenerContext.Request.Url);
                    if (baseUri == null)
                        throw new InvalidOperationException(string.Format(
                            "Unable to locate base URI for request: {0}", listenerContext.Request.Url));
                    var context = await _engine.HandleHttp(listenerContext, baseUri, cancellationToken).ConfigureAwait(false);
                    context.Dispose();
                }
            }
            catch (NotSupportedException)
            {
                listenerContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                listenerContext.Response.Close();
            }
            catch (Exception)
            {
                listenerContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                listenerContext.Response.Close();
            }
        }
    }
}
