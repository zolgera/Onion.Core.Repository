using Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repository.Query
{
    public class QueryOptions : IQueryOptions
    {
        IDictionary<string, object> Filter { set; get; }
        public string Sort { set; get; }
        public string SortOrder { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int StartRecord { get; set; }
        public int RecordCount { get; set; }
    }
}
