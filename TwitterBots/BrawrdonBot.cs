using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pyxcell;

namespace TwitterBots
{

  
    public class BrawrdonBot : TwitterBot
    {

        public BrawrdonBot(HttpClient client) : base(client, Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN"),  Environment.GetEnvironmentVariable("BRAWRDONBOT_CONSUMER_KEY_SECRET"), Environment.GetEnvironmentVariable("BRAWRDONBOT_OAUTH_TOKEN_SECRET"))
        {
        }

        public override async Task<JObject> PostTweet(string status, string replyToScreenName = null, string replyToStatusId = null, string mediaBase64 = null)
        {
            var generateMediaBase64 = await UploadImage(status);
           
            return await base.PostTweet(status, mediaBase64: generateMediaBase64);
        }
        

        private async Task<string> UploadImage(string status)
        {
            const string url = "https://upload.twitter.com/1.1/media/upload.json";
            var generator = new CommandGenerator();
            generator.Generate(status);

            var base64Image =  generator.DrawToBase64();
            var requestData = new SortedDictionary<string, string> { { "media_data", base64Image } };

            Authenticate(url, requestData);

            var content = new FormUrlEncodedContent(requestData);
            
            var response = await _client.PostAsync(url, content);
            var responseData = await response.Content.ReadAsStringAsync();
            var responseDataJson = (JObject)JsonConvert.DeserializeObject(responseData);

            return responseDataJson.Value<string>("media_id_string");
        }

    }
}