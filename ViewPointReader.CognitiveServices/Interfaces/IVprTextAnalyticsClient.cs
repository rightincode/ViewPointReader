using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViewPointReader.CognitiveServices.Interfaces
{
    public interface IVprTextAnalyticsClient
    {
        Task<List<string>> ExtractKeyPhrasesAsync(string feedDescription);
    }
}
