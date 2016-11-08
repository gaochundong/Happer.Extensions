using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Happer.TestHttpServer
{
    public interface IHybridEngine : IEngine
    {
        Task HandleWebSocket(HttpListenerContext httpContext, HttpListenerWebSocketContext webSocketContext, CancellationToken cancellationToken);
    }
}
