using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Dinosaur.Bots;
using Newtonsoft.Json.Linq;

namespace Dinosaur.Server
{
    internal static class Listener
    {
        private static HttpListener _listener;
        private static HttpClient _client;

        private static void Main(string[] args)
        {
            // TODO: Add exceptions because this broke when I tried to ping
            _client = new HttpClient();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:15101/");
            _listener.Start();
            Console.WriteLine("Listening... Press enter to stop");

            _listener.BeginGetContext(ListenerCallback, null);
            Console.ReadLine();
        }

        private static void ListenerCallback(IAsyncResult ar)
        {

            var context = _listener.EndGetContext(ar);
            var request = context.Request;
            var requestUrl = request.RawUrl.ToLower();
            var response = context.Response;
            JObject responseMessage = new JObject(
                           new JProperty("status", 400),
                           new JProperty("reason", "Invalid request"));

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
                        responseMessage = ProcessTwitterRequest(request, requestUrl);
                    }
                }
            }
            else
            {
                responseMessage["status"] = 405;
                responseMessage["reason"] = "Method not allowed";
            }

            response.StatusCode = (int)responseMessage["status"];
            response.StatusDescription = (string)responseMessage["reason"];
            response.Close();
        }

        private static JObject ProcessTwitterRequest(HttpListenerRequest request, string requestUrl)
        {
            JObject responseMessage = new JObject(
                 new JProperty("status", 400),
                 new JProperty("reason", "Invalid request"));

            // Removes /twitter from the url request to easily check what kind of request this is
            requestUrl = requestUrl.Remove(0, 9);

            // Request to post a tweet, could potentially have more bots
            if (requestUrl.StartsWith("post/"))
            {
                requestUrl = requestUrl.Remove(0, 5);
                if (requestUrl.Equals("brawrdonbot/"))
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        var brawrdonBot = new TwitterBot(_client, API.Twitter.BrawrdonBot.CosumerKey, API.Twitter.BrawrdonBot.OauthToken, API.Twitter.BrawrdonBot.CosumerKeySecret, API.Twitter.BrawrdonBot.OauthTokenSecret);

                        var requestBody = JObject.Parse(reader.ReadToEnd());

                        if (requestBody["message"] != null)
                        {
                            responseMessage = brawrdonBot.PostTweet(requestBody["message"].ToString()).Result;
                        }
                        else
                        {
                            responseMessage["reason"] = "Invalid JSON";
                        }
                    }
                }
            }
            return responseMessage;
        }
    }
}