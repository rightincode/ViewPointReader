using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ViewPointReader.CognitiveServices.Interfaces;
using ViewPointReader.CognitiveServices.Models;

namespace ViewPointReader.CognitiveServices.UnitTests.Searching
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
