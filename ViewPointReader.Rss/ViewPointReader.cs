using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader.Feeds;
using ViewPointReader.Rss.Interfaces;
using ViewPointReader.Web;
using ViewPointReader.Web.Interfaces;
using FeedItem = CodeHollow.FeedReader.FeedItem;

namespace ViewPointReader.Rss
{
    public class ViewPointReader : IViewPointReader
    {
        private readonly IVprWebSearchClient _vprWebSearchClient = new VprWebSearchClient("62212ab381824133b4f2dfbeef5ddfb7");

        public Task<List<FeedItem>> LoadSubscribedFeeds()
        {
            throw new NotImplementedException();
        }

        public async Task<List<FeedItem>> SearchForFeeds(string queryText)
        {
            var results = new List<FeedItem>();

            var webSearchResults = await _vprWebSearchClient.SearchAsync(queryText);

            //webSearchResults.ForEach(async x =>
            //{
            //    var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(x.Url);

            //    //results.AddRange(htmlFeedLinks.Select(htmlFeedLink => new FeedItem {Title = htmlFeedLink.Title, Link = htmlFeedLink.Url}));

            //    foreach (var htmlFeedLink in htmlFeedLinks)
            //    {
            //        results.Add(new FeedItem
            //        {
            //            Title = htmlFeedLink.Title,
            //            Link = htmlFeedLink.Url
            //        });
            //    }
            //});

            foreach (var vprWebSearchResult in webSearchResults)
            {
                var htmlFeedLinks = await FeedReader.GetFeedUrlsFromUrlAsync(vprWebSearchResult.Url);

                results.AddRange(htmlFeedLinks.Select(htmlFeedLink => new FeedItem {Title = htmlFeedLink.Title, Link = htmlFeedLink.Url}));  
            }

            return results;
        }
    }
}
