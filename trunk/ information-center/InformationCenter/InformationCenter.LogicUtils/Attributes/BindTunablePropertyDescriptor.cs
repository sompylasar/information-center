using System;
using System.ComponentModel;
using System.Drawing;

namespace LogicUtils.Attributes
{
    /// <summary> Описатель связываемого и настраиваемого свойства объекта </summary>
    /// <remarks> Поддерживает настройку связывания, выравнивание, коэффициент заполнения и т.д. </remarks>
    [Description("Описатель связываемого и настраиваемого свойства объекта")]
    public sealed class BindTunablePropertyDescriptor : PropertyDescriptor
    {
        #region Поля

        /// <summary> Описатель свойства </summary>
        private readonly PropertyDescriptor m_property;
        /// <summary> Класс для изменения значений атрибутов </summary>
        private readonly OverrideHelper m_overHelper;

        #endregion

        #region Конструктор

        /// <summary> Конструктор </summary>
        /// <param name="property"> свойство </param>
        /// <remarks> Свойство должно иметь тип ReflectPropertyDescriptor </remarks>
        public BindTunablePropertyDescriptor(PropertyDescriptor property) : base(property)
        {
            m_property = property;
            m_overHelper = new OverrideHelper(this);
        }

        #endregion

        #region Свойства 

        /// <summary> Получить (установить) тип выравнивания </summary>
        /// <value> тип выравнивания </value>
        public ContentAlignment Alignment
        {
            get { return (((HorizontalTextAlignmentAttribute)base.Attributes[typeof(HorizontalTextAlignmentAttribute)]).Alignment); }
            set
            {
                if (this.Alignment != value)
                    m_overHelper.AddAttribute(new HorizontalTextAlignmentAttribute(value));
            }
        }
        /// <summary> Получить (установить) коэффициент заполнения </summary>
        /// <value> коэффициент заполнения </value>
        public float FillWeight
        {
            get { return (((FillWeightAttribute)base.Attributes[typeof(FillWeightAttribute)]).Weight); }
            set
            {
                if (this.FillWeight != value)
                    m_overHelper.AddAttribute(new FillWeightAttribute(value));
            }
        }
        /// <summary> Получить (установить) индекс </summary>
        /// <value> индекс </value>
        public int Index
        {
            get { return (((DisplayIndexAttribute)base.Attributes[typeof(DisplayIndexAttribute)]).Index); }
            set
            {
                if (this.Index != value)
                    m_overHelper.AddAttribute(new DisplayIndexAttribute(value));
            }
        }
        /// <summary> Получить (установить) признак видимости </summary>
        /// <value> признак </value>
        public bool Visible
        {
            get { return (((VisibleAttribute)base.Attributes[typeof(VisibleAttribute)]).Visible); }
            set
            {
                if (this.Visible != value)
                    m_overHelper.AddAttribute(new VisibleAttribute(value));
            }
        }
        /// <summary> Получить (установить) режим вывода </summary>
        /// <value> режим </value>
        public AutoSizeModes AutoSizeMode
        {
            get { return (((AutoSizeModeAttribute)base.Attributes[typeof(AutoSizeModeAttribute)]).AutoSizeMode); }
            set
            {
                if (this.AutoSizeMode != value)
                    m_overHelper.AddAttribute(new AutoSizeModeAttribute(value));
            }
        }
        /// <summary> Получить тип объекта, свойства которого используются для связывания </summary>
        /// <value> тип </value>
        public override Type ComponentType { get { return (m_property.ComponentType); } }
        /// <summary> Получить признак свойства только для чтения </summary>
        /// <value> true если свойство только для чтения, в противном случае - false. </value>
        public override bool IsReadOnly { get { return (m_property.IsReadOnly); } }
        /// <summary> Получить тип свойства </summary>
        /// <value> тип </value>
        public override Type PropertyType { get { return (m_property.PropertyType); } }

        #endregion

        #region Методы

        /// <summary> Сброс свойства в значение по-умолчанию </summary>
        /// <param name="component"> компонент </param>
        public override void ResetValue(object component) { m_property.ResetValue(component); }
        /// <summary> Получить признак возможности установки свойства в значение по-умолчанию </summary>
        /// <param name="component"> компонент </param>
        /// <returns> true если возможно, в противном случае - false </returns>
        public override bool CanResetValue(object component) { return (m_property.CanResetValue(component)); }
        /// <summary> Получить признак сериализации свойства </summary>
        /// <param name="component"> компонент </param>
        /// <returns> true если свойство необходимо сериализовать, в противном случае - false </returns>
        public override bool ShouldSerializeValue(object component) { return (m_property.ShouldSerializeValue(component)); }
        /// <summary> Получить значение свойства </summary>
        /// <param name="component"> компонент </param>
        /// <returns> значение </returns>
        public override object GetValue(object component) { return (m_property.GetValue(component)); }
        /// <summary> Установить значение свойства </summary>
        /// <param name="component"> компонент </param>
        /// <param name="value"> значение </param>
        public override void SetValue(object component, object value) { m_property.SetValue(component, value); }
        /// <summary> Заполнить атрибуты из указанного описателя свойства </summary>
        /// <param name="property"> описатель свойства </param>
        public bool FillAttributes(FakeBindTunablePropertyDescriptor property)
        {
            if (m_property.Name == property.PropertyName)
            {
                this.Alignment = property.Alignment;
                this.AutoSizeMode = property.AutoSizeMode;
                this.FillWeight = property.FillWeight;
                this.Index = property.Index;
                this.Visible = property.Visible;
                return (true);
            }
            return (false);
        }

        #endregion

        #region Вспомогательные классы

        /// <summary> Вспомогательный класс для изменения атрибутов настраиваемого свойства </summary>
        [Serializable]
        [Description("Вспомогательный класс для изменения атрибутов настраиваемого свойства")]
        public sealed class OverrideHelper
        {
            /// <summary> Настраиваемое свойство объекта данных </summary>
            private readonly BindTunablePropertyDescriptor m_property;

            #region Конструктор

            /// <summary> Конструктор с описателем свойства </summary>
            /// <param name="property"> описатель настраиваемого свойства </param>
            public OverrideHelper(BindTunablePropertyDescriptor property) 
            {
                if (property == null)
                    throw new ArgumentNullException("property", "Не указан описатель свойства");
                m_property = property;
            }

            #endregion

            #region Методы

            /// <summary> Добавить (изменить значение) атрибут </summary>
            /// <param name="attribute"> атрибут </param>
            public void AddAttribute(OverridableAttribute attribute)
            {
                if (!ChangeAttributeValue(attribute))
                    this.AddNewAttribute(attribute);
            }
            /// <summary> Получить имя свойства </summary>
            /// <value> имя свойства </value>
            public string PropertyName { get { return (m_property.Name); } }
            /// <summary> Получить переопределенные атрибуты </summary>
            /// <value> массив атрибутов </value>
            /// <remarks> Возвращает массив атрибутов, которые изменили свое значение. Массив может быть пустым </remarks>
            public OverridableAttribute[] Overridable { get { return (new OverridableAttributeCollection(m_property.AttributeArray).GetDifferences(m_property.ComponentType, m_property.Name)); } }

            #endregion

            #region Внутренние методы

            /// <summary> Добавить новый атрибут </summary>
            /// <param name="attribute"> атрибут </param>
            private void AddNewAttribute(OverridableAttribute attribute)
            {
                if (attribute != null)
                {
                    Attribute[] oldAttributes = m_property.AttributeArray;
                    Attribute[] newAttributes = new Attribute[oldAttributes.Length + 1];
                    oldAttributes.CopyTo(newAttributes, 0);
                    newAttributes[oldAttributes.Length] = attribute;
                    m_property.AttributeArray = newAttributes;
                }
            }
            /// <summary> Изменить значение атрибута </summary>
            /// <param name="attribute"> атрибут </param>
            /// <returns> true, если успешно, в противном случае(атрибут не найден) - false </returns>
            private bool ChangeAttributeValue(OverridableAttribute attribute)
            {
                if (attribute != null)
                {
                    System.Type attributeType = attribute.GetType();
                    Attribute[] oldAttributes = m_property.AttributeArray;
                    for (int i = 0; i < oldAttributes.Length; i++)
                    {
                        if (attributeType.Equals(oldAttributes[i].GetType()))
                        {
                            // меняем, если значения не совпадают
                            if (!attribute.Equals(oldAttributes[i]))
                                oldAttributes[i] = attribute;
                            // всегда возвращаем TRUE
                            return (true);
                        }
                    }
                }
                // атрибут не найден
                return (false);
            }

            #endregion
        }

        #endregion
    }
}