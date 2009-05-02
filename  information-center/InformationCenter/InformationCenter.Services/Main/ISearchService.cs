using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public interface ISearchService
    {
        DocumentView GetDocument(Guid ID);
        //string[] GetFileNames(bool WithExtensions);
        FieldView[] GetFields();
        object GetValue(FieldValueView Value);
        object[] GetValuesOfField(FieldView FieldView);
        DocDescriptionView[] Query(SearchRequest Request);
    }

}