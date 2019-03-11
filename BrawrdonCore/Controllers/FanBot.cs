using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TwitterBots;

namespace BrawrdonCore.Controllers
{
    public class FanBotController : Controller
    {

        private readonly FanBot _fanBot;
        private readonly OAuthService _oAuthService;

        
        public FanBotController(FanBot fanBot, OAuthService oAuthService )
        {
            _fanBot = fanBot;
            _oAuthService = oAuthService;

        }

        
        [HttpGet("[controller]/register")]
        public async Task<IActionResult> Register()
        {
            var result = await _fanBot.RequestToken();
            
            if (!result.ContainsKey("oauth_callback_confirmed"))
                return BadRequest();

            if (!_oAuthService._requests.TryAdd(result["oauth_token"], result["oauth_token_secret"])) 
                return BadRequest();

            if (!bool.TryParse(result["oauth_callback_confirmed"], out var callbackConfirmed))
                return BadRequest();

            if (callbackConfirmed)
                return Redirect($"https://api.twitter.com/oauth/authorize?oauth_token={result["oauth_token"]}");

            return BadRequest();
        }
        
                
        [HttpGet("[controller]/callback")]        
        public async Task<IActionResult> Callback([FromQuery(Name = "oauth_token")] string oauth_token, [FromQuery(Name = "oauth_verifier")] string oauth_verifier)
        {
            _fanBot.OauthToken = oauth_token;
            _fanBot.OauthTokenSecret = _oAuthService._requests[oauth_token];
            var result = await _fanBot.AccessToken(oauth_verifier);

            if (!_oAuthService._authorisation.TryAdd(result["user_id"], new OAuth(result["oauth_token"], result["oauth_token_secret"])))
                return BadRequest();

            _fanBot.OauthToken = result["oauth_token"];
            _fanBot.OauthTokenSecret = result["oauth_token_secret"];
            _fanBot.PostTweet("This tweet came from me using a 3-legged OAuth request.");

            return Ok("It worked!");

        }
        
        
    }
}
