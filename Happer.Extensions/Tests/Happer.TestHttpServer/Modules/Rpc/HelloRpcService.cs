using System;
using System.Threading;
using System.Threading.Tasks;
using Happer.Rpc;

namespace Happer.TestHttpServer
{
    public class HelloRpcService : RpcService<HelloRequest, HelloResponse>
    {
        public override Task<HelloResponse> Execute(HelloRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new HelloResponse
                {
                    Result = string.Format("[{0}] Hello, {1}.",
                        DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff"),
                        request.Name),
                });
        }
    }
}
