using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// ��������� ������� ��� ������ ����������.
    /// </summary>
    public interface ISearchService
    {

        /// <summary>
        /// �������� ��� ������������� �����
        /// </summary>
        /// <returns>������ �������������</returns>
        FieldView[] GetFields();

        /// <summary>
        /// �������� �������� ����
        /// </summary>
        /// <param name="Value">������������� �������� ����</param>
        /// <returns>���� ��������</returns>
        object GetValue(FieldValueView Value);

        /// <summary>
        /// �������� ��� �������� ���������� ����
        /// </summary>
        /// <param name="FieldView">������������� ����, ��� �������� ����� �������� ��������</param>
        /// <returns>������ ��������</returns>
        object[] GetValuesOfField(FieldView FieldView);

        /// <summary>
        /// ����� � ���������
        /// </summary>
        /// <param name="Request">��������� ������</param>
        /// <returns>������ ��������� ������������� �������� ����������, ��������������� ���������� �������</returns>
        DocDescriptionView[] Query(SearchRequestView Request);

    }

}