using System;
using System.Net;
using System.Threading;

namespace Brawrdon.Messenger
{
    class Program
    {
        private static HttpListener listener;

        static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:15001/");
            listener.Start();
            Console.WriteLine("Listening... Press enter to stop");

            listener.BeginGetContext(ListenerCallback, null);
            Console.ReadLine();
        }

        private static void ListenerCallback(IAsyncResult ar)
        {
            Random random = new Random();
            HttpListenerContext context = listener.EndGetContext(ar);
            listener.BeginGetContext(ListenerCallback, null);
            Thread.Sleep(random.Next(1, 7) * 1000);
            Console.WriteLine(context.Request.ContentLength64);
            Console.WriteLine("Listening again..");

        }
    }
}