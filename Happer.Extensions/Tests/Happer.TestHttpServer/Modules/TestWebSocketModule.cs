using System;
using System.Text;
using System.Threading.Tasks;
using Happer.WebSockets;

namespace Happer.TestHttpServer
{
    public class TestWebSocketModule : WebSocketModule
    {
        public TestWebSocketModule()
            : base(@"/test")
        {
        }

        public override async Task ReceiveTextMessage(WebSocketTextMessage message)
        {
            Console.Write(string.Format("WebSocket session [{0}] received Text --> ", message.Session.RemoteEndPoint));
            Console.WriteLine(string.Format("{0}", message.Text));

            await message.Session.Send(message.Text);
        }

        public override async Task ReceiveBinaryMessage(WebSocketBinaryMessage message)
        {
            var text = Encoding.UTF8.GetString(message.Buffer, message.Offset, message.Count);
            Console.Write(string.Format("WebSocket session [{0}] received Binary --> ", message.Session.RemoteEndPoint));
            if (message.Count < 1024 * 1024 * 1)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.WriteLine("{0} Bytes", message.Count);
            }

            await message.Session.Send(message.Buffer, message.Offset, message.Count);
        }
    }
}
