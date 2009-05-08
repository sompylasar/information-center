using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Интерфейс сервиса для поиска документов.
    /// </summary>
    public interface ISearchService
    {

        /// <summary>
        /// получить все представления полей
        /// </summary>
        /// <returns>массив представлений</returns>
        FieldView[] GetFields();

        /// <summary>
        /// получить значение поля
        /// </summary>
        /// <param name="Value">представление значения поля</param>
        /// <returns>само значение</returns>
        object GetValue(FieldValueView Value);

        /// <summary>
        /// получить все значения некоторого поля
        /// </summary>
        /// <param name="FieldView">представление поля, для которого нужно получить значения</param>
        /// <returns>массив значений</returns>
        object[] GetValuesOfField(FieldView FieldView);

        /// <summary>
        /// поиск в хранилище
        /// </summary>
        /// <param name="Request">поисковый запрос</param>
        /// <returns>массив найденных представлений описаний документов, удовлетворяющих поисковому запросу</returns>
        DocDescriptionView[] Query(SearchRequestView Request);

    }

}