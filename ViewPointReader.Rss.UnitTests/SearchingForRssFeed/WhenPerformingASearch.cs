using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ViewPointReader.Rss.UnitTests.SearchingForRssFeed
{
    [TestClass]
    public class WhenPerformingASearch
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var reader = new ViewPointReader();

            var results = await reader.SearchForFeeds("business success");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
