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

            if (!_oAuthService.Secrets.TryAdd(result["oauth_token"], result["oauth_token_secret"])) 
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
            //ToDo: Look at HttpClient Factories
            var registeredFanBot = new FanBot(new HttpClient(), oauth_token, _oAuthService.Secrets[oauth_token]);
            
            var result = await registeredFanBot.AccessToken(oauth_verifier);

            if (!_oAuthService.Authorisations.TryAdd(result["user_id"], new OAuth(result["oauth_token"], result["oauth_token_secret"])))
                return BadRequest();

            registeredFanBot.OauthToken = result["oauth_token"];
            registeredFanBot.OauthTokenSecret = result["oauth_token_secret"];

            return Ok("You've been added to the dictionary");

        }
        
        
    }
}
