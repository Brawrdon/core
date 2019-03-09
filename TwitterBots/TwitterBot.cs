using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwitterBots
{
    public abstract class TwitterBot : ITwitterBot
    {
        protected readonly HttpClient _client;

        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly string _consumerKey;
        private readonly string _oauthToken;
        private readonly string _consumerKeySecret;
        private readonly string _oauthTokenSecret;


        protected TwitterBot(HttpClient client, string consumerKey, string oauthToken, string consumerKeySecret, string oauthTokenSecret)
        {
            _client = client;
            _consumerKey = consumerKey;
            _oauthToken = oauthToken;
            _consumerKeySecret = consumerKeySecret;
            _oauthTokenSecret = oauthTokenSecret;
        }


        public async Task<JObject> PostTweet(string status, string replyToScreenName = null, string replyToStatusId = null, string mediaBase64 = null)
        {
            const string url = "https://api.twitter.com/1.1/statuses/update.json";

            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Tweet requires a status.", status);

            var requestData = new SortedDictionary<string, string> {{"status", status}};

            if (!string.IsNullOrEmpty(replyToScreenName))
            {
                requestData[status] = $"@{replyToScreenName} {status}";
                requestData.Add("in_reply_to_status_id", replyToStatusId);
            }

            if (!string.IsNullOrEmpty(mediaBase64))
            {
                requestData.Add("media_ids", mediaBase64);
            }

            Authenticate(url, requestData);
            var content = new FormUrlEncodedContent(requestData);

            var response = await _client.PostAsync(url, content);

            var tweet = JObject.Parse(await response.Content.ReadAsStringAsync());

            return JObject.FromObject(new {status = response.StatusCode, tweetId = tweet.Value<string>("id_str")});
        }


        /// <summary>
        /// Generates the Authorization headers.
        /// </summary>
        /// <param name="url">The api end point.</param>
        /// <param name="requestData">The Json data sent in the body,</param>
        protected void Authenticate(string url, SortedDictionary<string, string> requestData)
        {
            var oauthNonce = GenerateNonce();

            // Sets up all the stuff Twitter needs to authenticate the request.
            var data = new SortedDictionary<string, string>
            {
                {"oauth_consumer_key", _consumerKey},
                {"oauth_nonce", oauthNonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()},
                {"oauth_token", _oauthToken},
                {"oauth_version", "1.0"}
            };

            foreach (var item in requestData)
            {
                data.Add(item.Key, item.Value);
            }

            data.Add("oauth_signature", GenerateSignature(data, url, _consumerKeySecret, _oauthTokenSecret));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", GenerateOauth(data));
        }

        /// <summary>
        /// Twitter requires that each request has a randomly generated nonce. This just takes 32 random values from alphabet and uses that as the nonce.
        /// </summary>
        /// <returns>The randomly generated nonce.</returns>
        private static string GenerateNonce()
        {
            var oauthNonce = "";
            var random = new Random();

            for (var i = 0; i < 32; i++)
            {
                oauthNonce += Alphabet[random.Next(0, Alphabet.Length)];
            }

            return oauthNonce;
        }

        /// <summary>
        /// Generates the signature. It first creates the "parameter string", then uses that to create the "base key" and then generates the signing key.
        /// The signing key is used with the base key to perform a HMACSHA1 has, that is then base64 encoded.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <param name="url">API endpoint.</param>
        /// <param name="consumerKeySecret">Consumer key secret</param>
        /// <param name="oauthTokenSecret">Oauth token secret</param>
        /// <returns>The generated signature.</returns>
        private static string GenerateSignature(SortedDictionary<string, string> data, string url,
            string consumerKeySecret, string oauthTokenSecret)
        {
            var parameterString = GenerateParameterString(data);

            var baseKey = GenerateBaseKey(parameterString, url);

            var signingKey = Uri.EscapeDataString(consumerKeySecret) + "&" + Uri.EscapeDataString(oauthTokenSecret);


            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey)))
            {
                return Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(baseKey)));
            }
        }

        /// <summary>
        /// Generates the parameter string by taking all the oauth and other values and joining them into Twitter's format.
        /// It percent encodes all the key values.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated parameter string.</returns>
        private static string GenerateParameterString(SortedDictionary<string, string> data)
        {
            return string.Join("&",
                data.Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))));
        }

        /// <summary>
        /// Generates the base key by encoding joining the request type, the url and the parameter string. It percent encodes all the values.
        /// </summary>
        /// <param name="parameterString">The generated parameter string</param>
        /// <param name="url">API endpoint</param>
        /// <returns>The generated base key.</returns>
        private static string GenerateBaseKey(string parameterString, string url)
        {
            return "POST&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(parameterString);
        }

        /// <summary>
        /// Takes all the data starting with oauth and creates an OAuth Authorization header.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated OAuth</returns>
        private static string GenerateOauth(SortedDictionary<string, string> data)
        {
            return string.Join(", ", data.Where(kvp => kvp.Key.StartsWith("oauth")).Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))).OrderBy(kvp => kvp));
        }
    }
}