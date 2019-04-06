using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewPointReader.Web.Interfaces;
using ViewPointReader.Web.Models;

namespace ViewPointReader.Web
{
    public class WebSearchClient : IWebSearchClient
    {
        public Task<List<WebSearchUrlResult>> SearchAsync(string query, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
