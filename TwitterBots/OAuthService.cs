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
        private const string _authorisationsFileName = "authorisations.txt";

        public OAuthService(IApplicationLifetime lifetime)
        {
            
            lifetime.ApplicationStopping.Register(SaveToDisk);

            Secrets = new ConcurrentDictionary<string, string>();

            if (File.Exists(_authorisationsFileName))
            {
                var authorisations = File.ReadAllText(_authorisationsFileName);
                Authorisations = JsonConvert.DeserializeObject<ConcurrentDictionary<string, OAuth>>(authorisations);
                File.Delete(_authorisationsFileName);
            }
            else
            {
                Authorisations = new ConcurrentDictionary<string, OAuth>();
            }

        }

        private void SaveToDisk()
        {
            File.WriteAllText(_authorisationsFileName, JsonConvert.SerializeObject(Authorisations));           
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