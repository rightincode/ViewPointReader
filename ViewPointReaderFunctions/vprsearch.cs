using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace ViewPointReaderFunctions
{
    public static class Vprsearch
    {
        [FunctionName("Vprsearch")]
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
                    processingTasks.Add(ProcessSearchResult(vprWebSearchResult));
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
            
            return new OkObjectResult(JsonConvert.SerializeObject(results
                , new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }));
        }

        private static async Task<List<Feed>> ProcessSearchResult(VprWebSearchResult vprWebSearchResult)
        {
            var results = new List<Feed>();
            
            try
            {
                var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url);

                if (htmlFeedLinks != null)
                {
                    var feedTasks = ProcessHtmlFeedLinks(htmlFeedLinks);
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
    }
}
