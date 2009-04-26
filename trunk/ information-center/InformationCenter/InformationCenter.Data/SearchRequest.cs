using System;
using System.Collections.Generic;

namespace InformationCenter.Data
{

    public class SearchRequest
    {

        public Dictionary<string, object> Fields { get; protected set; }

        public SearchRequest()
        {
            Fields = new Dictionary<string, object>();
        }

        public void Validate()
        {
            if (!Fields.ContainsKey("Author") || !Fields.ContainsKey("Title"))
                throw new ApplicationException("Поле названия либо поле автора должно быть заполнено");
        }
    }

}