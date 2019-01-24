using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrawrdonBot;
using Microsoft.Extensions.DependencyInjection;

namespace BrawrdonCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {

        private readonly ITwitterBot twitterBot;


        //ToDo: Look at collections for more than one Twitter bot
        public TwitterController(ITwitterBot twitterBot)
        {
            this.twitterBot = twitterBot;

        }

        [HttpPost("brawrdonbot")]
        public async Task<ActionResult> PostBrawrdonBot([FromBody] Tweet tweet)
        {
            var response = await twitterBot.PostTweet(tweet.Message);
            if (response.Value<int>("status") == 200)
            {
                return Ok(tweet.Message);
            }

            return new StatusCodeResult(500);
        }
    }
}
