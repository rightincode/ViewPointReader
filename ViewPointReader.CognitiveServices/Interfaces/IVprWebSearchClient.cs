using System.Collections.Generic;
using System.Threading.Tasks;
using ViewPointReader.CognitiveServices.Models;

namespace ViewPointReader.CognitiveServices.Interfaces
{
    public interface IVprWebSearchClient
    {
        Task<List<VprWebSearchResult>> SearchAsync(string query);
    }
}
