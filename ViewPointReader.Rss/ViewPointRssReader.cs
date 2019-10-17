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
                //var uri = new System.Uri(_searchUri + queryText + " blog");
                var uri = new System.Uri(_searchUri + queryText);

                var responseString = await _httpClient.GetStringAsync(uri);
                results = JsonConvert.DeserializeObject<List<Feed>>(responseString
                , new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }
    }
}
