using System;
using System.Collections.Generic;
using System.ComponentModel;
using LogicUtils;

namespace InformationCenter.Data
{

    /// <summary>
    /// Запрос поиска.
    /// </summary>
    public class SearchRequest
    {

        #region Поля

        private SearchMode mode = SearchMode.And;
        private List<SearchItem> items = new List<SearchItem>();

        #endregion

        #region Конструкторы

        public SearchRequest() { }

        #endregion

        #region Свойства

        /// <summary>
        /// режим поиска
        /// </summary>
        [DefaultValue(typeof(SearchMode), "SearchMode.And")]
        public SearchMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// коллекция критериев поиска
        /// </summary>
        public List<SearchItem> Items { get { return items; } }

        #endregion

    }

}