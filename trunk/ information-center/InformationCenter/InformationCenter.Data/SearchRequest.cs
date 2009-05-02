using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace InformationCenter.Data
{

    /// <summary>
    /// ������ ������.
    /// </summary>
    public class SearchRequest
    {

        #region ����

        private SearchMode mode = SearchMode.And;
        private List<SearchItem> items = new List<SearchItem>();

        #endregion

        #region ������������

        public SearchRequest() { }

        #endregion

        #region ��������

        /// <summary>
        /// ����� ������
        /// </summary>
        [DefaultValue(typeof(SearchMode), "SearchMode.And")]
        public SearchMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// ��������� ��������� ������
        /// </summary>
        public List<SearchItem> Items { get { return items; } }

        #endregion

    }

    /// <summary>
    /// ����� ������.
    /// </summary>
    [Description("����� ������")]
    public enum SearchMode
    {

        /// <summary>
        /// �
        /// </summary>
        [Description("�")]
        And,

        /// <summary>
        /// ���
        /// </summary>
        [Description("���")]
        Or

    }

}