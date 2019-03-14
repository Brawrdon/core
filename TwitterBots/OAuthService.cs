using System.Collections.Concurrent;

namespace TwitterBots
{
    public class OAuthService
    {
        public readonly ConcurrentDictionary<string, string> Secrets;
        public readonly ConcurrentDictionary<string, OAuth> Authorisations;

        public OAuthService()
        {
            Secrets = new ConcurrentDictionary<string, string>();
            Authorisations = new ConcurrentDictionary<string, OAuth>();
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