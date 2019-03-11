using System.Collections.Concurrent;

namespace TwitterBots
{
    public class OAuthService
    {
        public readonly ConcurrentDictionary<string, string> _requests;
        public readonly ConcurrentDictionary<string, OAuth> _authorisation;

        public OAuthService()
        {
            _requests = new ConcurrentDictionary<string, string>();
            _authorisation = new ConcurrentDictionary<string, OAuth>();
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