using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwitterBots
{
    public interface ITwitterBot
    {
        Task<JObject> PostTweet(string status);
        void SetOnlineStatus(bool status);
        
    }
}