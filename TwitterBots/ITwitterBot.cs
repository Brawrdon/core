using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwitterBots
{
    public interface ITwitterBot
    {
        Task<JObject> PostTweet(string status, string replyToScreenName = null, string replyToStatusId = null, string mediaBase64 = null);
    }
}