using System;
using System.Buffers.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Dinosaur.Bots
{
    public class Twitter
    {
        private readonly HttpClient client = new HttpClient();
        private const string Alpbabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static string oauthNonce;
        private static string consumer;
        private static string token;
        private static string oauth;
        private static string signature;


        public Twitter()
        {
        
            // Generate oauth_nonce
            Random random = new Random();
            for (int i = 0; i < 32; i++)
            {
                oauthNonce += Alpbabet[random.Next(0, Alpbabet.Length)];
            }

            // Percent encode stuff
            oauthNonce = EscapeString(oauthNonce);
            consumer = EscapeString(API.Twitter.CosumerKey);
            token = EscapeString(API.Twitter.OauthToken);
            
            
            // Generate Oauth

            oauth = GenerateOauth();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Oauth",
                "");
            
            
//            string auth = client.GetStringAsync();

        }

        private static string EscapeString(string data)
        {
            return Uri.EscapeDataString(data);

        }

        private static string GenerateOauth()
        {
            return "oauth_nonce=\"" + oauthNonce + "\",oauth_signature=" + signature + "";
            
        }
    }
}