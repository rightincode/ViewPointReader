using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;

namespace ViewPointReaderFunctions
{
    public static class Vprscorefeed
    {
        [FunctionName("vprscorefeed")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var feed = JsonConvert.DeserializeObject<FeedSubscription>(requestBody);

            feed.RecommendationScore = await ScoreFeedAsync(feed);

            return new OkObjectResult(JsonConvert.SerializeObject(feed));
        }

        private static async Task<float> ScoreFeedAsync(IFeedSubscription feedSubscription)
        {
            float score = 0;

            var requestUrl = new Uri("https://viewpointreaderwebapi.azurewebsites.net/api/subscriptions/scorefeed");

            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
            {
                using (var stringContent =
                    new StringContent(JsonConvert.SerializeObject(feedSubscription), Encoding.UTF8, "application/json"))
                {
                    var httpClient = new HttpClient();
                    request.Content = stringContent;

                    var responseMessage = await httpClient.SendAsync(request);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        float.TryParse(await responseMessage.Content.ReadAsStringAsync(), out score);
                    }
                }
            }

            return score;
        }
    }
}
