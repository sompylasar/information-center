using System;
using System.Collections.Generic;

namespace InformationCenter.Data
{

    public class SearchResults
    {

        public List<SearchResultItem> Items { get; protected set; }

        
        public SearchResults(IEnumerable<SearchResultItem> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            Items = new List<SearchResultItem>(items);
        }

        public SearchResults() : this(new SearchResultItem[0]) { }

    }

    public class SearchResultItem
    {

        public Int64 ID { get; protected set; }
        public string Header { get; protected set; }
        public string Url { get; protected set; }

        public SearchResultItem(Int64 id, string header, string url)
        {
            if (header == null) throw new ArgumentNullException("header");
            if (url == null) throw new ArgumentNullException("url");

            ID = id;
            Header = header;
            Url = url;
        }

    }

}