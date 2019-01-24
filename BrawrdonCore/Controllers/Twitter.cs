using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrawrdonBot;

namespace BrawrdonCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        // GET api/values
        [HttpPost("brawrdonbot")]
        public ActionResult PostBrawrdonBot([FromBody]Tweet tweet)
        {

            return Ok(tweet.Message);
        }
    }
}
