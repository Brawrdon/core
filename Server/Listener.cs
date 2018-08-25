using System;
using System.IO;
using System.Net;

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

            // TODO: Send response back if statement not reached
            if (context.Request.LocalEndPoint.Port == 9801 && context.Request.HttpMethod.ToUpper().Equals("POST"))
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    Bots.Twitter.PostTweet(reader.ReadToEnd());
                }
            }
        }
    }
}