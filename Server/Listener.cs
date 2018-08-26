using System;
using System.IO;
using System.Net;

namespace Dinosaur.Server
{
    internal static class Listener
    {
        private static HttpListener _listener;

        private static void Main(string[] args)
        {
            // TODO: Add exceptions because this broke when I tried to ping
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:15001/");

            _listener.Start();
            Console.WriteLine("Listening... Press enter to stop");

            _listener.BeginGetContext(ListenerCallback, null);
            Console.ReadLine();
        }

        private static void ListenerCallback(IAsyncResult ar)
        {
            var request = _listener.EndGetContext(ar).Request;
            var requestUrl = request.RawUrl.ToLower();

            // Appends a backslash to the end of the request URL to make sure checks are done properly
            if (!requestUrl.EndsWith("/"))
            {
                requestUrl += "/";
            }

            _listener.BeginGetContext(ListenerCallback, null);

            // TODO: Send response back if statement not reached
            if (request.HttpMethod.ToUpper().Equals("POST"))
            {
                // Parse Twitter requests
                if (requestUrl.StartsWith("/twitter"))
                {
                    if (request.ContentType.Equals("application/json"))
                    {
                        ProcessTwitterRequest(request, requestUrl);
                    }
                }
            }

            GenerateResponse();
        }

        private static void ProcessTwitterRequest(HttpListenerRequest request, string requestUrl)
        {
            // Removes /twitter from the url request to easily check what kind of request this is
            requestUrl = requestUrl.Remove(0, 9);
            
            // Request to post a tweet
            if (requestUrl.Equals("post/"))
            {                        
                // TODO: Check if the Json contains the message parameter, ignore everything else
                using (var reader = new StreamReader(request.InputStream))
                {
                    Bots.Twitter.PostTweet(reader.ReadToEnd());
                }               
            }
        }
        
        private static void GenerateResponse()
        {
            throw new NotImplementedException();
        }

    }
}