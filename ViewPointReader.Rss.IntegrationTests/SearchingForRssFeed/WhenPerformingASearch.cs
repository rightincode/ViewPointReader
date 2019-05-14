using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ViewPointReader.Rss.IntegrationTests.SearchingForRssFeed
{
    [TestClass]
    public class WhenPerformingASearch
    {
        [TestMethod]
        [Ignore]
        public async Task AndTheSearchIsValid()
        {
            var reader = new ViewPointRssReader();

            var results = await reader.SearchForFeedsAsync("business success");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
