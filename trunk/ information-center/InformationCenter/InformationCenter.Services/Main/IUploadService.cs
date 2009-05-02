using System;
using System.IO;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    public interface IUploadService
    {
        int MaxFileSizeInBytes { get; }
        TemplateView[] GetTemplates();
        FieldView[] GetFieldsOfTemplate(TemplateView TemplateView);
        void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView);
        bool AddTemplate(string Name, IEnumerable<FieldView> FieldViews);
        Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength);
        bool AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues);
    }

}