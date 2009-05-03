using System;
using System.Collections.Generic;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление шаблона документа.
    /// </summary>
    public class TemplateView : ViewItem, IEquatable<TemplateView>
    {

        #region Конструкторы

        internal TemplateView(Template Template) : base(Template) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект
        /// </summary>
        internal Template Template { get { return entity as Template; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Template.ID; } }

        /// <summary>
        /// название шаблона
        /// </summary>
        public string Name { get { return Template.Name; } }

        /// <summary>
        /// дата создания шаблона
        /// </summary>
        public DateTime CreationDate { get { return Template.CreationDate; } }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as TemplateView); }

        public bool Equals(TemplateView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return Name; }

        #endregion

    }

}