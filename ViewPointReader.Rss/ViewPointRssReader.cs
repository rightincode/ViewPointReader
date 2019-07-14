using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewPointReader.CognitiveServices.Interfaces;
using ViewPointReader.Rss.Interfaces;
using FeedItem = CodeHollow.FeedReader.FeedItem;

namespace ViewPointReader.Rss
{
    public class ViewPointRssReader : IViewPointRssReader
    {
        private readonly IVprWebSearchClient _vprWebSearchClient;
        private readonly IVprTextAnalyticsClient _vprTextAnalyticsClient;

        public ViewPointRssReader(IVprWebSearchClient vprWebSearchClient, IVprTextAnalyticsClient vprTextAnalyticsClient)
        {
            _vprWebSearchClient = vprWebSearchClient;
            _vprTextAnalyticsClient = vprTextAnalyticsClient;
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
                var webSearchResults = await _vprWebSearchClient.SearchAsync(queryText + " blog");

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
                        results.AddRange(intermediateResult);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        private async Task<List<Feed>> ProcessSearchResult(CognitiveServices.Models.VprWebSearchResult vprWebSearchResult)
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
