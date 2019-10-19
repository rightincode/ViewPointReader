using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ViewPointReader.CognitiveServices;

namespace ViewPointReaderFunctions
{
    public static class Vprkeyphraseextract
    {
        [FunctionName("vprkeyphraseextract")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string content = data?.content ?? string.Empty;

            if (string.IsNullOrEmpty(content))
            {
                return new NoContentResult();
            }

            var textAnalyticsClient = new VprTextAnalyticsClient("0ae5b7dd8d584b3196516ce807b9aa4e");
            var keyPhraseResults = await textAnalyticsClient.ExtractKeyPhrasesAsync(content);

            return new OkObjectResult(keyPhraseResults);
        }
    }
}
