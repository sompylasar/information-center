using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    /// <summary>
    /// Bynthatqc cервисf для работы с описаниями документов и шаблонами описаний.
    /// </summary>
    public interface IDocumentDescriptionService
    {

        #region Get

        /// <summary>
        /// получить все представления существующих шаблонов описаний
        /// </summary>
        /// <returns>массив представлений шаблонов</returns>
        TemplateView[] GetTemplates();

        /// <summary>
        /// получить все поля шаблона
        /// </summary>
        /// <param name="TemplateView">представление шаблона, для которого нужно получить поля</param>
        /// <returns>массив представлений полей шалона</returns>
        FieldView[] GetFieldsOfTemplate(TemplateView TemplateView);

        /// <summary>
        /// получить все представления типов полей
        /// </summary>
        /// <returns>массив представлений типов полей хранилища</returns>
        FieldTypeView[] GetFieldTypes();

        /// <summary>
        /// получить представление описания по идентификатору описания
        /// </summary>
        /// <param name="descriptionId">идентификатор описания</param>
        /// <returns>представление описания</returns>
        DocDescriptionView GetDescription(Guid descriptionId);

        #endregion

        #region Add

        /// <summary>
        /// добавить новое поле в хранилище
        /// </summary>
        /// <param name="Name">имя поля</param>
        /// <param name="Type">представление типа поля</param>
        /// <param name="Nullable">признак того, что поле может не иметь значения</param>
        /// <param name="Order">приоритет поля</param>
        /// <returns>идентификатор добавленного поля</returns>
        Guid AddField(string Name, FieldTypeView Type, bool Nullable, int Order);

        /// <summary>
        /// добавить новый шаблон в хранилище
        /// </summary>
        /// <param name="Name">имя шаблона</param>
        /// <param name="FieldViews">набор представлений полей шаблона</param>
        /// <returns>идентификатор добавленного шаблона описания</returns>
        Guid AddTemplate(string Name, IEnumerable<FieldView> FieldViews);

        /// <summary>
        /// добавить поле к шаблону
        /// </summary>
        /// <param name="TemplateID">идентификатор шаблона</param>
        /// <param name="FieldID">идентификатор поля</param>
        /// <exception cref="">в случае провала генерирует исключение</exception>
        void AddFieldToTemplate(Guid TemplateID, Guid FieldID);

        #endregion

        #region Modify

        /// <summary>
        /// переименовать поле
        /// </summary>
        /// <param name="Field">представление поля</param>
        /// <param name="NewName">новое имя поля</param>
        void RenameField(FieldView Field, string NewName);

        /// <summary>
        /// переименовать шаблон
        /// </summary>
        /// <param name="Template">представление шаблона</param>
        /// <param name="NewName">новое имя шаблона</param>
        void RenameTemplate(TemplateView Template, string NewName);

        /// <summary>
        /// изменить приоритет представления поля
        /// </summary>
        /// <param name="Field">представление поля</param>
        /// <param name="Order">новый приоритет</param>
        void ChangeFieldOrder(FieldView Field, int Order);

        #endregion

        #region Remove/Delete

        /// <summary>
        /// удалить поле из шаблона
        /// </summary>
        /// <param name="TemplateView">представление шаблона, из которого удаляется поле</param>
        /// <param name="FieldView">представление поля</param>
        void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView);

        /// <summary>
        /// удалить шаблон из хранилища
        /// </summary>
        /// <param name="Template">представление удаляемого шаблона</param>
        void DeleteTemplate(TemplateView Template);

        /// <summary>
        /// удалить поле из хранилища
        /// </summary>
        /// <param name="Field">представление удаляемого поля</param>
        void DeleteField(FieldView Field);

        /// <summary>
        /// удалить описание документа
        /// </summary>
        /// <param name="DocumentDescription">представление описания</param>
        void DeleteDocumentDescription(DocDescriptionView DocumentDescription);

        #endregion
    }

}
