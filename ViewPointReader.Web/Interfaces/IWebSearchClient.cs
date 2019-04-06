using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewPointReader.Web.Models;

namespace ViewPointReader.Web.Interfaces
{
    public interface IWebSearchClient
    {
        Task<List<WebSearchUrlResult>> SearchAsync(string query, int offset, int count);
    }
}
