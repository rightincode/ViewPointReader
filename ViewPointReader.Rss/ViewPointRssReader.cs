using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeHollow.FeedReader.Feeds;
using ViewPointReader.Rss.Interfaces;
using ViewPointReader.Web;
using ViewPointReader.Web.Interfaces;
using FeedItem = CodeHollow.FeedReader.FeedItem;

namespace ViewPointReader.Rss
{
    public class ViewPointRssReader : IViewPointRssReader
    {
        private readonly IVprWebSearchClient _vprWebSearchClient = new VprWebSearchClient("62212ab381824133b4f2dfbeef5ddfb7");

        public Task<List<FeedItem>> LoadSubscribedFeeds()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Feed>> SearchForFeeds(string queryText)
        {
            var results = new List<Feed>();
            try
            {
                var webSearchResults = await _vprWebSearchClient.SearchAsync(queryText);

                //var htmlFeedLinksTasks = new List<Task<IEnumerable<HtmlFeedLink>>>();

                //foreach (var vprWebSearchResult in webSearchResults)
                //{
                //    try
                //    {
                //        htmlFeedLinksTasks.Add(FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url));
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e);
                //        //throw;
                //    }
                //}

                //var htmlFeedLinkEnumerableArray = await Task.WhenAll(htmlFeedLinksTasks);

                //foreach (var htmlFeedLinks in htmlFeedLinkEnumerableArray)
                //{
                //    try
                //    {
                //        var feedTasks = ProcessHtmlFeedLinks(htmlFeedLinks);
                //        var feeds = await Task.WhenAll(feedTasks);

                //        foreach (var feed in feeds)
                //        {
                //            if (results.All(x => x.Link != feed.Link) && feed.Items.Count > 0)
                //            {
                //                results.Add(feed);
                //            }
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e);
                //        throw;
                //    }

                //}

                var stopwatch = Stopwatch.StartNew();

                foreach (var vprWebSearchResult in webSearchResults)
                {
                    try
                    {
                        stopwatch.Start();

                        var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url);

                        stopwatch.Stop();
                        Console.WriteLine(
                            $"Time to retrieve feeds from {vprWebSearchResult.Url} was {stopwatch.ElapsedMilliseconds}ms");
                        Console.WriteLine();
                        stopwatch.Restart();
                       
                        var feedTasks = ProcessHtmlFeedLinks(htmlFeedLinks);
                        var feeds = await Task.WhenAll(feedTasks);

                        stopwatch.Stop();
                        Console.WriteLine($"Time to read feeds for {vprWebSearchResult.Url} was {stopwatch.ElapsedMilliseconds}ms ");
                        Console.WriteLine();

                        foreach (var feed in feeds)
                        {
                            if (results.All(x => x.Link != feed.Link) && feed.Items.Count > 0)
                            {
                                results.Add(feed);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
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
                    Console.WriteLine(e);
                    //throw;
                }
            }

            return feedTasks;
        }
    }
}
