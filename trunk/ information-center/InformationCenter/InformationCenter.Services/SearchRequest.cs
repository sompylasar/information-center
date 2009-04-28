using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InformationCenter.Services
{

    public class SearchRequest
    {

        #region Поля

        private SearchMode mode = SearchMode.And;
        private List<SearchItem> items = new List<SearchItem>();

        #endregion

        #region Конструкторы

        public SearchRequest()
        {
        }

        #endregion

        #region Свойства

        [DefaultValue(typeof(SearchMode), "SearchMode.And")]
        public SearchMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public List<SearchItem> Items { get { return items; } }

        #endregion

    }

    [Description("Режим поиска")]
    public enum SearchMode
    {

        [Description("И")]
        And,

        [Description("Или")]
        Or

    }

}