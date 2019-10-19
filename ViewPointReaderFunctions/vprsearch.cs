using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ViewPointReader.CognitiveServices;
using ViewPointReader.CognitiveServices.Models;
using ViewPointReader.Core.Interfaces;
using ViewPointReader.Core.Models;

namespace ViewPointReaderFunctions
{
    public static class Vprsearch
    {
        private const string ExtractKeyPhrasesUri = "https://viewpointreaderfunctions.azurewebsites.net/api/vprkeyphraseextract";
        private const string ScoreFeedUri = "https://viewpointreaderfunctions.azurewebsites.net/api/vprscorefeed";
        private static readonly HttpClient FuncHttpClient = new HttpClient();

        [FunctionName("vprsearch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string searchText = req.Query["searchtext"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            searchText = searchText ?? data?.name;
            
            var searchClient = new VprWebSearchClient("62212ab381824133b4f2dfbeef5ddfb7");
            var webSearchResults = await searchClient.SearchAsync(searchText);

            var results = new List<Feed>();
            if (webSearchResults == null)
                return new OkObjectResult(JsonConvert.SerializeObject(results
                    , new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    }));
            
            var processingTasks = new List<Task<List<Feed>>>();
            foreach (var vprWebSearchResult in webSearchResults)
            {
                try
                {
                    processingTasks.Add(ProcessSearchResultAsync(vprWebSearchResult));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            var intermediateResults = await Task.WhenAll(processingTasks);
            foreach (var intermediateResult in intermediateResults)
            {
                if (intermediateResult.Count > 0)
                {
                    intermediateResult.ForEach(x =>
                    {
                        if (results.All(y => y.Link != x.Link))
                        {
                            results.Add(x);
                        }
                    });
                }
            }

            var buildFeedResultsTasks = results.Select(BuildFeedSubscriptionResponse).ToList();
            var feeds = await Task.WhenAll(buildFeedResultsTasks);


            return new OkObjectResult(JsonConvert.SerializeObject(feeds));
        }

        private static async Task<List<Feed>> ProcessSearchResultAsync(VprWebSearchResult vprWebSearchResult)
        {
            var results = new List<Feed>();

            if (vprWebSearchResult.Url.Contains("feedspot.com")) return results;
            
            try
            {
                var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url);
                var feedLinks = htmlFeedLinks.ToList();

                if (feedLinks.Any())
                {
                    var feedTasks = ProcessHtmlFeedLinks(feedLinks);
                    var feeds = await Task.WhenAll(feedTasks);

                    foreach (var feed in feeds)
                    {
                        if (results.Any(x => x.Link == feed.Link) || feed.Items.Count <= 0) continue;
                        if (string.IsNullOrEmpty(feed.Title) || string.IsNullOrEmpty(feed.Description)) continue;
                        feed.Title = feed.Title.Trim();
                        results.Add(feed);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        private static IEnumerable<Task<Feed>> ProcessHtmlFeedLinks(IEnumerable<HtmlFeedLink> htmlFeedLinks)
        {
            var feedTasks = new List<Task<Feed>>();

            foreach (var htmlFeedLink in htmlFeedLinks)
            {
                try
                {
                    feedTasks.Add(FeedReader.ReadAsync(htmlFeedLink.Url));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return feedTasks;
        }

        private static async Task<IFeedSubscription> BuildFeedSubscriptionResponse(Feed feed)
        {
            var feedSubscription =  new FeedSubscription
            {
                Title = feed.Title,
                Description = feed.Description,
                Url = feed.Link,
                ImageUrl = feed.ImageUrl,
                LastUpdated = feed.LastUpdatedDate,
                SubscribedDate = DateTime.Now,
                FeedItems = new List<VprFeedItem>()
            };
            
            if (!string.IsNullOrEmpty(feedSubscription.Description))
            {
                feedSubscription.KeyPhrases = await ExtractKeyPhrasesAsync(feedSubscription.Description);
            }

            feedSubscription = await ScoreFeed(feedSubscription);

            if (feed.Items.Count == 0) return feedSubscription;

            foreach (var feedItem in feed.Items)
            {
                var vprFeedItem = new VprFeedItem
                {
                    Author = feedItem.Author,
                    Categories = feedItem.Categories,
                    Content = feedItem.Content,
                    Description = feedItem.Description,
                    Link = feedItem.Link,
                    PublishingDate = feedItem.PublishingDate,
                    PublishingDateString = feedItem.PublishingDateString,
                    Title = feedItem.Title
                };

                feedSubscription.FeedItems.Add(vprFeedItem);
            }

            return feedSubscription;
        }

        public static async Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription)
        {
            var results = new List<string>();
            var content = new VprKeyPhraseContent
            {
                Content = feedDescription
            };

            try
            {
                var uri = new Uri(ExtractKeyPhrasesUri);
                
                var response = await FuncHttpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(content),
                    Encoding.UTF8, "application/json"));

                results = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        public static async Task<FeedSubscription> ScoreFeed(FeedSubscription feed)
        {
            var result = feed;

            try
            {
                var uri = new Uri(ScoreFeedUri);
                
                var response = await FuncHttpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(feed),
                    Encoding.UTF8, "application/json"));

                result = JsonConvert.DeserializeObject<FeedSubscription>(await response.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
    }
}
