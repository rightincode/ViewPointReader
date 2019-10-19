using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ViewPointReader.Core.Models;
using ViewPointReader.Rss.Interfaces;
using FeedItem = CodeHollow.FeedReader.FeedItem;

namespace ViewPointReader.Rss
{
    public class ViewPointRssReader : IViewPointRssReader
    {
        private const string SearchUri = "https://viewpointreaderfunctions.azurewebsites.net/api/Vprsearch?searchtext=";
        private const string ExtractKeyPhrasesUri = "https://viewpointreaderfunctions.azurewebsites.net/api/vprkeyphraseextract";
        private readonly HttpClient _httpClient;

        public ViewPointRssReader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        //TODO:This will be moved to the azure vprsearch function (score/extract in one search call)
        public async Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription)
        {
            var results = new List<string>();
            var content = new VprKeyPhraseContent
            {
                Content = feedDescription
            };

            try
            {
                var uri = new Uri(ExtractKeyPhrasesUri);

                var response = await _httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(content),
                    Encoding.UTF8, "application/json"));

                results = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
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
                var uri = new System.Uri(SearchUri + "'" + queryText + " rss" + "'");

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
