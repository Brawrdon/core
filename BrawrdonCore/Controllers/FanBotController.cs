using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitterBots;

namespace BrawrdonCore.Controllers
{
    [EnableCors]
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

            return Ok("User had been added!");

        }
        
        [HttpGet("[controller]/users")]        
        public IActionResult GetUsers()
        {
            // ToDo: Change to Authentication Required
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderStringValues))
                return Unauthorized();

            var authorizationHeader = authorizationHeaderStringValues.ToString();
            
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Basic")) 
                return Unauthorized();
            
            var token = authorizationHeader.Substring("Basic ".Length).Trim();

            if (token != Environment.GetEnvironmentVariable("FANBOT_ACCESS_TOKEN"))
                return Unauthorized();
            
            return Ok(JsonConvert.SerializeObject(_oAuthService.Authorisations));
        }
       
        
        
    }
}
