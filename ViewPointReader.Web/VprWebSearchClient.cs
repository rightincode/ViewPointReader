using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ViewPointReader.Web.Interfaces;
using ViewPointReader.Web.Models;

namespace ViewPointReader.Web
{
    public class VprWebSearchClient : IVprWebSearchClient
    {
        private readonly WebSearchClient _azureWebSearchApiClient;

        public VprWebSearchClient(string apiKey)
        {
            _azureWebSearchApiClient = new WebSearchClient(new ApiKeyServiceClientCredentials(apiKey));
        }

        public Task<List<VprWebSearchResult>> SearchAsync(string query)
        {
            return Task.Run(() => BuildSearchResults(_azureWebSearchApiClient.Web.SearchAsync(
                query,null,null,null,null,null,null,1,null,50,null,null,null,null,null, safeSearch:"Strict",null,null,null)));
        }

        private static List<VprWebSearchResult> BuildSearchResults(Task<SearchResponse> response)
        {
            return response.Result.WebPages.Value?.Select(webPage =>
                new VprWebSearchResult
                {
                    Id = webPage.Id,
                    Name = webPage.Name,
                    DisplayUrl = webPage.DisplayUrl,
                    Url = webPage.Url,
                    Description = webPage.Description,
                    Snippet = webPage.Snippet,
                    ThumbnailUrl = webPage.ThumbnailUrl
                }).ToList();            
        }
    }
}
