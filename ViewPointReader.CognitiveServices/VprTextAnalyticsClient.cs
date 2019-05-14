using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using ViewPointReader.CognitiveServices.Interfaces;

namespace ViewPointReader.CognitiveServices
{
    public class VprTextAnalyticsClient : IVprTextAnalyticsClient
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public VprTextAnalyticsClient(string apiKey)
        {
            _textAnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = "https://eastus2.api.cognitive.microsoft.com"
            };
        }

        public async Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription)
        {
            var input = new MultiLanguageBatchInput(new List<MultiLanguageInput>
            {
                new MultiLanguageInput("en", "1", feedDescription)
            });

            var keyPhraseResult = await _textAnalyticsClient.KeyPhrasesAsync(false, input);

            var results = new List<string>();

            foreach (var document in keyPhraseResult.Documents)
            {
                results.AddRange(document.KeyPhrases);
            }

            return results;
        }
    }
}
