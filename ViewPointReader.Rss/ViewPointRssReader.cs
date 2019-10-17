using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodeHollow.FeedReader.Feeds;
using Newtonsoft.Json;
using ViewPointReader.CognitiveServices.Interfaces;
using ViewPointReader.CognitiveServices.Models;
using ViewPointReader.Rss.Interfaces;
using FeedItem = CodeHollow.FeedReader.FeedItem;

namespace ViewPointReader.Rss
{
    public class ViewPointRssReader : IViewPointRssReader
    {
        private readonly string _searchUri =
            "https://viewpointreaderfunctions.azurewebsites.net/api/Vprsearch?searchtext=";
        private readonly HttpClient _httpClient;
        private readonly IVprTextAnalyticsClient _vprTextAnalyticsClient;

        public ViewPointRssReader(IVprTextAnalyticsClient vprTextAnalyticsClient, HttpClient httpClient)
        {
            _vprTextAnalyticsClient = vprTextAnalyticsClient;
            _httpClient = httpClient;
        }

        public async Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription)
        {
            return await _vprTextAnalyticsClient.ExtractKeyPhrasesAsync(feedDescription);
        }

        public Task<List<FeedItem>> LoadSubscribedFeeds()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Feed>> SearchForFeedsAsync(string queryText)
        {
            var results = new List<Feed>();
 
            try
            {
                var uri = new System.Uri(_searchUri + queryText + " blog");

                var responseString = await _httpClient.GetStringAsync(uri);
                results = JsonConvert.DeserializeObject<List<Feed>>(responseString
                , new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                #region Not used
                //var processingTasks = new List<Task<List<Feed>>>();

                //foreach (var vprWebSearchResult in webSearchResults)
                //{
                //    try
                //    {
                //        processingTasks.Add(ProcessSearchResult(vprWebSearchResult));
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }
                //}

                //var intermediateResults = await Task.WhenAll(processingTasks);

                //foreach (var intermediateResult in intermediateResults)
                //{
                //    if (intermediateResult.Count > 0)
                //    {
                //        intermediateResult.ForEach(x =>
                //        {
                //            if (results.All(y => y.Link != x.Link))
                //            {
                //                results.Add(x);
                //            }
                //        });
                //    }
                //}
                

                #endregion
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        private async Task<List<Feed>> ProcessSearchResult(VprWebSearchResult vprWebSearchResult)
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

        private IEnumerable<Task<Feed>> ProcessHtmlFeedLinks(IEnumerable<HtmlFeedLink> htmlFeedLinks)
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
