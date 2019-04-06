using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewPointReader.Web.Interfaces;
using ViewPointReader.Web.Models;

namespace ViewPointReader.Web.UnitTests.Searching
{
    [TestClass]
    public class WhenPerformingASearch
    {
        private IWebSearchClient _webSearchClient;

        [TestInitialize]
        public void TestStart()
        {
            var webSearchClientMock = new Mock<IWebSearchClient>();
            webSearchClientMock.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult(new List<WebSearchUrlResult>
                {
                    new WebSearchUrlResult()
                }));

            _webSearchClient = webSearchClientMock.Object;

        }

        [TestMethod]
        public async Task AndTheQueryIsBlank()
        {
            List<WebSearchUrlResult> results = await _webSearchClient.SearchAsync("", 0, 0);

            Assert.IsTrue(results.Count > 0);
        }
    }
}
