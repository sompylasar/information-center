using System;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    public class SearchResult
    {

        public List<SearchResultItem> Items { get; protected set; }

        
        public SearchResult(IEnumerable<SearchResultItem> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            Items = new List<SearchResultItem>(items);
        }

        public SearchResult() : this(new SearchResultItem[0]) { }

    }

    

}