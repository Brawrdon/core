using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BrawrdonCore.Services
{
    public class WebSocketService
    {
        public readonly ConcurrentBag<WebSocket> WebSockets;

        public WebSocketService()
        {
            WebSockets = new ConcurrentBag<WebSocket>();
        }
        public async Task WaitForCloseAsync(WebSocket webSocket)
        {
            var buffer = new byte[4096];
            try
            {
                do
                {
                    WebSocketReceiveResult response;

                    do
                    {
                        // Only listen to close messages
                        response = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (webSocket.State == WebSocketState.CloseReceived)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, response.CloseStatusDescription, CancellationToken.None);
                        }
                    } while (!response.EndOfMessage);


                } while (webSocket.State == WebSocketState.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception with WebSocket - Client probably closed too early.");
            }
            finally
            {
                webSocket.Dispose();
            }
        }
    }
}