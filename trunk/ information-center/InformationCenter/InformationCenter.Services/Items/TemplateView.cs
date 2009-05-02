using System;
using System.Collections.Generic;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class TemplateView : ViewItem
    {

        #region Конструкторы

        internal TemplateView(Template Template) : base(Template) { }

        #endregion

        #region Свойства

        public Guid ID { get { return Template.ID; } }

        internal Template Template { get { return entity as Template; } }

        public string Name { get { return Template.Name; } }

        public DateTime CreationDate { get { return Template.CreationDate; } }

        #endregion
    }

}