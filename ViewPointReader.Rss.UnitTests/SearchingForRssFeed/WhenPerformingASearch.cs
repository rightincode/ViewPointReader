using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using ViewPointReader.CognitiveServices.Interfaces;
using ViewPointReader.CognitiveServices.Models;

namespace ViewPointReader.Rss.UnitTests.SearchingForRssFeed
{
    [TestClass]
    public class WhenPerformingASearch
    {
        [TestMethod]
        public async Task AnyTheSearchIsValidNoResult()
        {
            var webSearchClientMock = new Mock<IVprWebSearchClient>();
            webSearchClientMock.Setup(ws => ws.SearchAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new List<VprWebSearchResult>());

            var textAnalyticsClientMock = new Mock<IVprTextAnalyticsClient>();
            //textAnalyticsClientMock.Setup(tac => tac.ExtractKeyPhrasesAsync(It.IsAny<string>()))
            //    .ReturnsAsync(() => new List<string>());

            var reader = new ViewPointRssReader(webSearchClientMock.Object, textAnalyticsClientMock.Object);

            var results = await reader.SearchForFeedsAsync("search text expect no result");

            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public async Task AnyTheSearchIsValidNoFeedFound()
        {
            var webSearchClientMock = new Mock<IVprWebSearchClient>();
            webSearchClientMock.Setup(ws => ws.SearchAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new List<VprWebSearchResult>
                {
                    new VprWebSearchResult
                    {
                        Description = "Business Success Blog",
                        DisplayUrl = "https://www.entrepreneur.com/article/246929",
                        Id = "resultId",
                        Name = "Business Success Blog",
                        Snippet = "This is a blog about business success",
                        ThumbnailUrl = "thumbnailUrl",
                        Url = "https://www.entrepreneur.com/article/246929"
                    }
                });

            var textAnalyticsClientMock = new Mock<IVprTextAnalyticsClient>();
            //textAnalyticsClientMock.Setup(tac => tac.ExtractKeyPhrasesAsync(It.IsAny<string>()))
            //    .ReturnsAsync(() => new List<string>());

            var reader = new ViewPointRssReader(webSearchClientMock.Object, textAnalyticsClientMock.Object);

            var results = await reader.SearchForFeedsAsync("business success");

            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public async Task AnyTheSearchIsValidFeedFound()
        {
            var webSearchClientMock = new Mock<IVprWebSearchClient>();
            webSearchClientMock.Setup(ws => ws.SearchAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new List<VprWebSearchResult>
                {
                    new VprWebSearchResult
                    {
                        Description = "description",
                        DisplayUrl = "https://www.socialmediaexaminer.com/10-top-business-blogs-and-why-they-are-successful/",
                        Id = "resultId",
                        Name = "name",
                        Snippet = "snippet",
                        ThumbnailUrl = "https://www.socialmediaexaminer.com/10-top-business-blogs-and-why-they-are-successful/",
                        Url = "https://www.socialmediaexaminer.com/10-top-business-blogs-and-why-they-are-successful/"
                    }
                });

            var textAnalyticsClientMock = new Mock<IVprTextAnalyticsClient>();
            //textAnalyticsClientMock.Setup(tac => tac.ExtractKeyPhrasesAsync(It.IsAny<string>()))
            //    .ReturnsAsync(() => new List<string>());

            var reader = new ViewPointRssReader(webSearchClientMock.Object, textAnalyticsClientMock.Object);

            var results = await reader.SearchForFeedsAsync("business success");

            Assert.IsTrue(results.Count > 0);
        }

    }
}
