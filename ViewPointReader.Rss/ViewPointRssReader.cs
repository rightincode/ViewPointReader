using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
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

                foreach (var vprWebSearchResult in webSearchResults)
                {
                    try
                    {
                        var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url);

                        foreach (var htmlFeedLink in htmlFeedLinks)
                        {
                            try
                            {
                                var feed = await FeedReader.ReadAsync(htmlFeedLink.Url);

                                if (results.All(x => x.Link != feed.Link) && feed.Items.Count > 0)
                                {
                                    results.Add(feed);
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            return results;
        }
    }
}
