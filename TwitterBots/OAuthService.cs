using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace TwitterBots
{
    public class OAuthService
    {
        public readonly ConcurrentDictionary<string, string> Secrets;
        public readonly ConcurrentDictionary<string, OAuth> Authorisations;

        public OAuthService(IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopping.Register(SaveToDisk);

            var authorisations = File.ReadAllText("authorisations.txt");
            Secrets = new ConcurrentDictionary<string, string>();
            Authorisations = JsonConvert.DeserializeObject<ConcurrentDictionary<string, OAuth>>(authorisations);
        }

        private void SaveToDisk()
        {
            File.WriteAllText("authorisations.txt", JsonConvert.SerializeObject(Authorisations));           
        }
    }


    public class OAuth
    {
        public readonly string OAuthToken;
        public readonly string OAuthTokenSecret;

        public OAuth(string oauthToken, string oauthTokenSecret)
        {
            OAuthToken = oauthToken;
            OAuthTokenSecret = oauthTokenSecret;
        }
        
    }
}