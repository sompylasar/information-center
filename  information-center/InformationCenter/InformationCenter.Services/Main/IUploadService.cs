using System;
using System.IO;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    public interface IUploadService
    {
        int MaxFileSizeInBytes { get; }
        Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength);
        bool AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues);
    }

}