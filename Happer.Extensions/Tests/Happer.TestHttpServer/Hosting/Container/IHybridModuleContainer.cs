using System;
using System.Collections.Generic;
using Happer.Http;
using Happer.WebSockets;

namespace Happer.TestHttpServer
{
    public interface IHybridModuleContainer : IModuleContainer
    {
        IEnumerable<WebSocketModule> GetAllWebSocketModules();
        WebSocketModule GetWebSocketModule(Type moduleType);
    }
}
