using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TwitterBots;

namespace BrawrdonCore.Controllers
{
    [Route("twitter/brawrdonbot")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        private readonly BrawrdonBot _brawrdonBot;

        public TwitterController(BrawrdonBot brawrdonBot)
        {
            _brawrdonBot = brawrdonBot;
        }

        public async Task<ActionResult> PostTweet([FromBody] Tweet tweet)
        {
            var response = await _brawrdonBot.PostTweet(tweet.Message);
            if (response.Value<int>("status") == 200)
            {
                return Ok(tweet.Message);
            }

            return new StatusCodeResult(500);
        }
    }
}
