using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;

namespace InformationCenter.Services
{

    /// <summary>
    /// Базовый класс для инкапсуляции объектов, используемых в Entity Framework.
    /// </summary>
    public abstract class ViewItem : INotifyPropertyChanging, INotifyPropertyChanged
    {

        #region Поля

        protected EntityObject entity = null;

        #endregion

        #region Конструкторы

        internal ViewItem(EntityObject Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity");
            entity = Entity;
        }

        #endregion

        #region События

        /// <summary>
        /// изменяется какое-то свойство
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { entity.PropertyChanging += value; }
            remove { entity.PropertyChanging -= value; }
        }

        /// <summary>
        /// свойство изменено
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { entity.PropertyChanged += value; }
            remove { entity.PropertyChanged -= value; }
        }

        #endregion

        #region Свойства

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return entity.ToString(); }

        #endregion

    }

}