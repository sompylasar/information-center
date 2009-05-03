using System;
using System.IO;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    /// <summary>
    /// ��������� ������� ��� ������� ���������� (upload).
    /// </summary>
    public interface IUploadService
    {

        /// <summary>
        /// ����������� ��������� ������ ������������ �� ������ ����� � ������
        /// </summary>
        int MaxFileSizeInBytes { get; }

        /// <summary>
        /// ��������� �������� �� ������
        /// </summary>
        /// <param name="Stream">����� ������ ���������</param>
        /// <param name="FileName">��������/��� ����� ���������</param>
        /// <param name="contentType">��� ��������� (�������)</param>
        /// <param name="ContentLength">������ ������������ �����</param>
        /// <returns>���������� ������������� ������������ ���������</returns>
        /// <exception cref="">� ������ ������� ������������ ������</exception>
        Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength);

        /// <summary>
        /// ���������� �������� � ������������� ���������
        /// </summary>
        /// <param name="DocumentID">������������� ���������</param>
        /// <param name="Name">��� ��������</param>
        /// <param name="FieldsWithValues">�������� ��������</param>
        /// <returns>true - �����, false - ������</returns>
        bool AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues);

    }

}