using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Web.Models;

namespace ViewPointReader.Web.Interfaces
{
    public interface IVprWebSearchClient
    {
        Task<List<VprWebSearchResult>> SearchAsync(string query);
    }
}
