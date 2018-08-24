using System;
using System.IO;
using System.Net;
using Dinosaur.Bots;

namespace Dinosaur.Server
{
    internal static class Listener
    {
        private static HttpListener _listener;
        private const string Twitter = "http://localhost:9801/";

        private static void Main(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:15001/");
            _listener.Prefixes.Add(Twitter);

            _listener.Start();
            Console.WriteLine("Listening... Press enter to stop");

            _listener.BeginGetContext(ListenerCallback, null);
            Console.ReadLine();
        }

        private static void ListenerCallback(IAsyncResult ar)
        {
            var context = _listener.EndGetContext(ar);
            _listener.BeginGetContext(ListenerCallback, null);

            if (context.Request.LocalEndPoint.Port == 9801)
            {
                new Twitter();
            }
            
            using(var reader = new StreamReader(context.Request.InputStream))
            {
                Console.WriteLine(reader.ReadToEnd());

            }

            
        }
    }
}
