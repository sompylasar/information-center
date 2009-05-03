using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    public interface IDocumentDescriptionService
    {

        #region Get

        TemplateView[] GetTemplates();
        FieldView[] GetFieldsOfTemplate(TemplateView TemplateView);

        #endregion

        #region Add

        void AddField(string Name, FieldTypeView Type);
        bool AddTemplate(string Name, IEnumerable<FieldView> FieldViews);
        void AddFieldToTemplate(Guid TemplateID, Guid FieldID);

        #endregion

        #region Modify

        void RenameField(FieldView Field, string NewName);
        void RenameTemplate(TemplateView Template, string NewName);

        #endregion

        #region Remove/Delete

        void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView);
        void DeleteTemplate(TemplateView Template);
        void DeleteField(FieldView Field);

        #endregion

    }

}
