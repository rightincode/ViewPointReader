using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using ViewPointReader.CognitiveServices;

namespace ViewPointReader.Rss.IntegrationTests.SearchingForRssFeed
{
    [TestClass]
    public class WhenPerformingASearch
    {
        [TestMethod]
        [Ignore]
        public async Task AndTheSearchIsValidWithResults()
        {
            var reader = new ViewPointRssReader(new VprWebSearchClient("62212ab381824133b4f2dfbeef5ddfb7"),
                new VprTextAnalyticsClient("0ae5b7dd8d584b3196516ce807b9aa4e"));

            var results = await reader.SearchForFeedsAsync("business success");

            Assert.IsTrue(results.Count > 0);
        }
        
    }
}
