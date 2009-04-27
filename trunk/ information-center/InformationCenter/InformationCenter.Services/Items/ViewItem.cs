using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;

namespace InformationCenter.Services
{

    public class ViewItem : INotifyPropertyChanging, INotifyPropertyChanged
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

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { entity.PropertyChanging += value; }
            remove { entity.PropertyChanging -= value; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { entity.PropertyChanged += value; }
            remove { entity.PropertyChanged -= value; }
        }

        #endregion

    }

}