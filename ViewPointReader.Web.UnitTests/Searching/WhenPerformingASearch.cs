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
        private IVprWebSearchClient _iVprWebSearchClient;

        [TestInitialize]
        public void TestStart()
        {
            var webSearchClientMock = new Mock<IVprWebSearchClient>();
            webSearchClientMock.Setup(x => x.SearchAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new List<VprWebSearchResult>
                {
                    new VprWebSearchResult()
                }));

            _iVprWebSearchClient = webSearchClientMock.Object;

        }

        [TestMethod]
        public async Task AndTheQueryIsBlank()
        {
            List<VprWebSearchResult> results = await _iVprWebSearchClient.SearchAsync("");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
