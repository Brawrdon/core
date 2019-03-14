using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pyxcell;

namespace TwitterBots
{
    public class FanBot : TwitterBot
    {

        public FanBot(HttpClient client) : base(client, Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN"),  Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY_SECRET"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN_SECRET"))
        {
        }
        
        public FanBot(HttpClient client, string oauthToken, string oauthTokenSecret) : base(client, Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY"), oauthToken, Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY_SECRET"), oauthTokenSecret)
        {
        }


        public async Task<Dictionary<string, string>> RequestToken()
        {
            const string url = "https://api.twitter.com/oauth/request_token";
            var requestData = new SortedDictionary<string, string> {{"oauth_callback", "http://localhost:5000/fanbot/callback"}};
            Authenticate(url, requestData);
            var content = new FormUrlEncodedContent(requestData);

            var response = await _client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error! Status code was: {response.StatusCode}");
           
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                return responseBody.Split('&').Select(value => value.Split('=')).ToDictionary(key => key[0], value => value[1]);
                    
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading response from Twitter. Response body was: {responseBody}");
                throw;
            }
       

        }

        public async Task<Dictionary<string, string>> AccessToken(string verify)
        {
            const string url = "https://api.twitter.com/oauth/access_token";
            var requestData = new SortedDictionary<string, string> {{"oauth_verifier", verify}};


            Authenticate(url, requestData);
            var content = new FormUrlEncodedContent(requestData);

            var response = await _client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error! Status code was: {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                return responseBody.Split('&').Select(value => value.Split('=')).ToDictionary(key => key[0], value => value[1]);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading response from Twitter. Response body was: {responseBody}");
                throw;
            }
        }

        public override async Task<JObject> PostTweet(string status, string replyToScreenName = null, string replyToStatusId = null, string mediaBase64 = null)
        {
          return await base.PostTweet(status);
        }

    }
}