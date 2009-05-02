using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    public interface IDocumentDescriptionService
    {
        TemplateView[] GetTemplates();
        FieldView[] GetFieldsOfTemplate(TemplateView TemplateView);
        void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView);
        bool AddTemplate(string Name, IEnumerable<FieldView> FieldViews);
        void AddFieldToTemplate(Guid TemplateID, Guid FieldID);
    }

}
