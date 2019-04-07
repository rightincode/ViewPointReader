using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewPointReader.Web.Interfaces;

namespace ViewPointReader.Web.IntegrationTests.Searching
{
    [TestClass]
    public class WhenPerformingASearch
    {
        private IVprWebSearchClient _vprWebSearchClient;
        private readonly string _apiKey = "62212ab381824133b4f2dfbeef5ddfb7";


        [TestMethod]
        public async Task AndTheSearchIsValid()
        {
            _vprWebSearchClient = new VprWebSearchClient(_apiKey);

            var results = await _vprWebSearchClient.SearchAsync("Xamarin");

            Assert.IsTrue(results.Count > 0);
        }
    }
}
