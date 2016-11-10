using System;
using System.Diagnostics;
using Happer.Rpc;
using Logrila.Logging.NLogIntegration;

namespace Happer.TestHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NLogLogger.Use();

            // ----------------------------------------------------------------------------------

            // POST : http://localhost:22222/rpc/hello
            // POST RAW DATA : {"Name":"Dennis" }
            // RETURN RESULT : {"Result":"Hello, Dennis."}
            Func<Type, object> rpcContainer = (t) =>
            {
                return new HelloRpcService();
            };
            var rpcServiceResolver = new RpcServiceResolver(rpcContainer);
            var rpc = new TestRpcModule(rpcServiceResolver);
            rpc.RegisterRpcService(rpcServiceResolver.GetRpcService<HelloRequest, HelloResponse>());

            // ----------------------------------------------------------------------------------

            var container = new TestContainer();
            container.AddModule(new TestModule());
            container.AddWebSocketModule(new TestWebSocketModule());
            container.AddModule(rpc);

            // ----------------------------------------------------------------------------------

            var bootstrapper = new HybridBootstrapper();
            var engine = bootstrapper.BootWith(container);

            // ----------------------------------------------------------------------------------

            string uri = "http://localhost:22222/";
            var host = new HybridSelfHost(engine, new Uri(uri));
            host.Start();
            Console.WriteLine("Server is listening on [{0}].", uri);

            //AutoNavigateTo(uri);

            // ----------------------------------------------------------------------------------

            Console.ReadKey();
            Console.WriteLine("Stopped. Goodbye!");
        }

        private static void AutoNavigateTo(string uri)
        {
            try
            {
                Process.Start(uri);
            }
            catch (Exception) { }
        }
    }
}
