using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BrawrdonCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrawrdonCore.Middlewares
{
    // ToDo: Expand to allow multiple WebSocket connections
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WebSocketService webSocketService)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                await _next(context);

            if (!AuthenticateWebSocketConnection(context))
                return;

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            webSocketService.WebSockets.Add(webSocket);
            webSocketService.WaitForCloseAsync(webSocket);
            await _next(context);
        }

        public static void MapWebSocket(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.AcceptWebSocket();
        }

        private static bool AuthenticateWebSocketConnection(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var accessToken))
                return false;
            
            // ToDo: Implement way more secure security measures
            return accessToken == Environment.GetEnvironmentVariable("BRAWRDON_CORE_WEBSOCKET_TOKEN");
        }
    }
    
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder AcceptWebSocket(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketMiddleware>();
        }
    }
   
}