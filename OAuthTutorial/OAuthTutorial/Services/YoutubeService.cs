using OAuthTutorial.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuthTutorial.Services
{
    public class YoutubeService
    {
        private const string YoutubeApiEndpoint = "https://www.googleapis.com/youtube/v3/channels";

        public static async Task<string> GetMyChannelIdAsync(string accessToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "mine", "true" }
            };

            var response = await HttpClientHelper.SendGetRequest<dynamic>(YoutubeApiEndpoint, queryParams, accessToken);

            var channelId = response.items[0].id;
            return channelId;
        }

        public static async Task UpdateChannelDescriptionAsync(string accessToken, string channelId, string newDescription)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "part", "brandingSettings" }
            };

            var body = new
            {
                id = channelId,
                brandingSettings = new
                {
                    channel = new
                    {
                        description = newDescription
                    }
                }
            };

            await HttpClientHelper.SendPutRequest(YoutubeApiEndpoint, queryParams, body, accessToken);
        }
    }
}
