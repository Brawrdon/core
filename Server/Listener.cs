using System;
using System.IO;
using System.Net;

namespace Server
{
    internal static class Listener
    {
        private static HttpListener _listener;
        private const string twitter = "http://localhost:9801/";

        private static void Main(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:15001/");
            _listener.Prefixes.Add(twitter);

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
                Console.WriteLine("Tweet");
            }
            
            using(var reader = new StreamReader(context.Request.InputStream))
            {
                Console.WriteLine(reader.ReadToEnd());

            }

        }
    }
}
