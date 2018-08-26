using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dinosaur.Bots
{
    public static class Twitter
    {
        private static readonly HttpClient Client = new HttpClient();
        private const string Alpbabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private const string Url = "https://api.twitter.com/1.1/statuses/update.json";

        /// <summary>
        /// This static method is the entry point. I felt made sense to be static because you just call the PostTweet method.
        ///
        /// API keys are stored in a file within the same namespace called API.cs under the Twitter class as 'public const string KeyName'. I've done this for security reasons.
        ///
        /// The method deals with generating the things required for the Twitter OAuth which is a whole load of encoding.
        /// </summary>
        /// <param name="status">The status to be tweeted.</param>
        public static async Task<string> PostTweet(string status)
        {
            var oauthNonce = GenerateNonce();

            // Sets up all the stuff Twitter needs to authneticate the request.
            var data = new SortedDictionary<string, string>
            {
                {"status", status},
                {"oauth_consumer_key", API.Twitter.BrawrdonBot.CosumerKey},
                {"oauth_nonce", oauthNonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()},
                {"oauth_token", API.Twitter.BrawrdonBot.OauthToken},
                {"oauth_version", "1.0"}
            };

            data.Add("oauth_signature", GenerateSignature(data));


            // Because the class is static and mutliple requests are being recieved asyncronously, we need to check
            // if the Authorization header is already set. If this isn't done it causes an "already set error", funnily enough.
            if (Client.DefaultRequestHeaders.Contains("Authorization"))
            {
                Client.DefaultRequestHeaders.Remove("Authorization");
            }

            Client.DefaultRequestHeaders.Add("Authorization", GenerateOauth(data));

            // Selects only the non-oauth key value pairs.
            var content = new FormUrlEncodedContent(data.Select(kvp => kvp).Where(kvp => !kvp.Key.StartsWith("oauth")));

            
            // Request is sent! Woo! (I added a small delay so that Twitter doesn't think two people are spamming)
            Thread.Sleep(500);
            var response = await Client.PostAsync("https://api.twitter.com/1.1/statuses/update.json", content);

            return GenerateResponse(response.StatusCode.ToString());


        }

        private static string GenerateResponse(string response)
        {
            throw new NotImplementedException();
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
                oauthNonce += Alpbabet[random.Next(0, Alpbabet.Length)];
            }

            return oauthNonce;
        }

        /// <summary>
        /// Deals with generating the signature. It first creates the "parameter string", then uses that to create the "base key" and then generates the signing key. The signing key is used with the base key to perform a HMACSHA1 has, that is then base64 encoded.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated signature.</returns>
        private static string GenerateSignature(SortedDictionary<string, string> data)
        {
            var parameterString = GenerateParameterString(data);
            var basekey = GenerateBaseKey(parameterString);
            var signingKey = Uri.EscapeDataString(API.Twitter.BrawrdonBot.CosumerKeySecret) +
                             "&" + Uri.EscapeDataString(API.Twitter.BrawrdonBot.OauthTokenSecret);


            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey)))
            {
                return Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(basekey)));
            }
        }

        /// <summary>
        /// Generates the parameter string by taking all the oauth and other values and joining them into Twitter's format. It percent encodes all the key values.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated parameter string.</returns>
        private static string GenerateParameterString(SortedDictionary<string, string> data)
        {
            return string.Join("&",
                data.Select(kvp =>
                    string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))));
        }

        /// <summary>
        /// Generates the base key by encoding joining the request type, the url and the parameter string. It percent encodes all the values.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated base key.</returns>
        private static string GenerateBaseKey(string parameterString)
        {
            return "POST&" +
                   Uri.EscapeDataString(Url) +
                   "&" +
                   Uri.EscapeDataString(parameterString);
        }

        /// <summary>
        /// Takes all the data starting with oauth and creates an OAuth Autherization header.
        /// </summary>
        /// <param name="data">The sorted dictionary with the oauth and other data.</param>
        /// <returns>The generated OAuth</returns>
        private static string GenerateOauth(SortedDictionary<string, string> data)
        {
            return "OAuth " + string.Join(", ",
                       data.Where(kvp => kvp.Key.StartsWith("oauth"))
                           .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key),
                               Uri.EscapeDataString(kvp.Value)))
                           .OrderBy(kvp => kvp));
        }
    }
}