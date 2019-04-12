using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ViewPointReader.Rss.UnitTests.SearchingForRssFeed
{
    [TestClass]
    public class WhenPerformingASearch
    {
        [TestMethod]
        [Ignore]
        public async Task AnyTheSearchIsValid()
        {
            var reader = new ViewPointRssReader();

            var results = await reader.SearchForFeeds("business success");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
