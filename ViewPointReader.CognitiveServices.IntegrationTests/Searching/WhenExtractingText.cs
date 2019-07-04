using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewPointReader.CognitiveServices.Interfaces;

namespace ViewPointReader.CognitiveServices.IntegrationTests.Searching
{
    [TestClass]
    public class WhenExtractingText
    {

        private IVprTextAnalyticsClient _vprTextAnalyticsClient;
        private readonly string _apiKey = "0ae5b7dd8d584b3196516ce807b9aa4e";

        [TestMethod]
        public async Task AndTheSuppliedTextHasKeyValues()
        {
            _vprTextAnalyticsClient = new VprTextAnalyticsClient(_apiKey);

            var results = await _vprTextAnalyticsClient.ExtractKeyPhrasesAsync("Xamarin");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
