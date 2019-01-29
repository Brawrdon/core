using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterBots;

namespace BrawrdonCore.Controllers
{
    [Route("/twitter/brawrdonbot")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        private readonly ITwitterBot _brawrdonBot;

        public TwitterController(BrawrdonBot brawrdonBot)
        {
            _brawrdonBot = brawrdonBot;
        }

        
        [HttpPost]
        public async Task<ActionResult> PostTweet([FromBody] Tweet tweet)
        {
            var response = await _brawrdonBot.PostTweet(tweet.Message);
            if (response.Value<int>("status") == 200)
                return Ok(tweet.Message);


            var result = new ObjectResult("There was an issue with starting BrawrdonBot.") {StatusCode = 500};

            return result;
        }
    }
}
