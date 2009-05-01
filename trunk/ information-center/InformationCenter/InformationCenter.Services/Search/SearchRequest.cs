using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InformationCenter.Services
{

    public class SearchRequest
    {

        #region ����

        private SearchMode mode = SearchMode.And;
        private List<SearchItem> items = new List<SearchItem>();

        #endregion

        #region ������������

        public SearchRequest()
        {
        }

        #endregion

        #region ��������

        [DefaultValue(typeof(SearchMode), "SearchMode.And")]
        public SearchMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public List<SearchItem> Items { get { return items; } }

        #endregion

    }

    [Description("����� ������")]
    public enum SearchMode
    {

        [Description("�")]
        And,

        [Description("���")]
        Or

    }

}