using System;
using System.IO;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    /// <summary>
    /// Интерфейс сервиса для заливки документов (upload).
    /// </summary>
    public interface IUploadService
    {

        /// <summary>
        /// максимально возможный размер загружаемого на сервер файла в байтах
        /// </summary>
        int MaxFileSizeInBytes { get; }

        /// <summary>
        /// загрузить документ на сервер
        /// </summary>
        /// <param name="Stream">поток данных документа</param>
        /// <param name="FileName">название/имя файла документа</param>
        /// <param name="contentType">тип документа (контент)</param>
        /// <param name="ContentLength">размер загружаемого файла</param>
        /// <returns>уникальный идентификатор загруженного документа</returns>
        /// <exception cref="">в случае неудачи генерируется ошибка</exception>
        Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength);

        /// <summary>
        /// добавление описания к существующему документу
        /// </summary>
        /// <param name="DocumentID">идентификатор документа</param>
        /// <param name="Name">имя описания</param>
        /// <param name="FieldsWithValues">элементы описания</param>
        /// <returns>true - успех, false - ошибка</returns>
        bool AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues);

    }

}