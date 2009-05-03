using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Запрос поиска.
    /// </summary>
    public class SearchRequestView
    {

        #region Поля

        private SearchMode mode = SearchMode.And;
        private List<SearchItemView> items = new List<SearchItemView>();

        #endregion

        #region Конструкторы

        public SearchRequestView() { }

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
        public List<SearchItemView> Items { get { return items; } }

        #endregion

        #region Методы

        internal SearchRequest ToInnerRequest()
        {
            SearchRequest result = new SearchRequest();
            result.Mode = Mode;
            result.Items.AddRange(Items.Select(i => new SearchItem(i.Field.Field, i.FieldValue)));
            return result;
        }

        #endregion

    }

}