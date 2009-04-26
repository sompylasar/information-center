using System;
using System.ComponentModel;
using System.Drawing;

namespace LogicUtils.Attributes
{
    /// <summary> Класс - имитация BindTunablePropertyDescriptor </summary>
    [Serializable]
    [Description("Облегченная версия BindTunablePropertyDescriptor")]
    public sealed class FakeBindTunablePropertyDescriptor
    {
        #region Поля

        /// <summary> Тип выравнивания </summary>
        private ContentAlignment m_alignment;
        /// <summary> Ширина </summary>
        private float m_fillWeight;
        /// <summary> Индекс </summary>
        private int m_index;
        /// <summary> Режим вывода </summary>
        private AutoSizeModes m_mode;
        /// <summary> Признак видимости </summary>
        private bool m_visible;
        /// <summary> Имя свойства </summary>
        private string m_name;
        /// <summary> Отображаемое имя свойства (выводимое пользователю) </summary>
        private string m_displayName;
        /// <summary> Описание свойства </summary>
        private string m_description;

        #endregion

        #region Конструкторы

        /// <summary> Конструктор для десериализации </summary>
        public FakeBindTunablePropertyDescriptor() { }
        /// <summary> Конструктор с именем свойства </summary>
        /// <param name="propertyName"> имя свойства </param>
        public FakeBindTunablePropertyDescriptor(string propertyName) { m_name = propertyName; }
        /// <summary> Конструктор со свойством </summary>
        /// <param name="property"> описатель свойства объекта </param>
        public FakeBindTunablePropertyDescriptor(BindTunablePropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property", "Не указан описатель свойства");
            else
            {
                this.PropertyName = property.Name;
                this.DisplayName = property.DisplayName;
                this.Description = property.Description;
                this.Alignment = property.Alignment;
                this.AutoSizeMode = property.AutoSizeMode;
                this.FillWeight = property.FillWeight;
                this.Index = property.Index;
                this.Visible = property.Visible;
            }
        }

        #endregion

        #region Свойства

        /// <summary> Получить (установить) тип выравнивания </summary>
        /// <value> тип выравнивания </value>
        [DisplayName("Тип выравнивания содержимого")]
        [Description("Выберите нужный тип выравнивания")]
        [UserDesignAttribute(true)]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ContentAlignment Alignment
        {
            get { return (m_alignment); }
            set { m_alignment = value; }
        }
        /// <summary> Получить (установить) коэффициент заполнения </summary>
        /// <value> коэффициент заполнения </value>
        [DisplayName("Коэффициент заполнения")]
        [Description("Введите нужный коэффициент заполнения")]
        [UserDesignAttribute(true)]
        public float FillWeight
        {
            get { return (m_fillWeight); }
            set { m_fillWeight = value; }
        }
        /// <summary> Получить (установить) индекс </summary>
        /// <value> индекс </value>
        [DisplayName("Номер столбца")]
        [Description("Укажите номер столбца")]
        [UserDesignAttribute(true)]
        public int Index
        {
            get { return (m_index); }
            set { m_index = value; }
        }
        /// <summary> Получить (установить) режим вывода </summary>
        /// <value> режим </value>
        [DisplayName("Тип автоширины")]
        [Description("Выберите нужное значение")]
        [UserDesignAttribute(true)]
        public AutoSizeModes AutoSizeMode
        {
            get { return (m_mode); }
            set { m_mode = value; }
        }
        /// <summary> Получить (установить) признак видимости </summary>
        /// <value> признак </value>
        [DisplayName("Вывод на экран")]
        [Description("Выберите нужное значение")]
        [UserDesignAttribute(true)]
        [TypeConverter(typeof(RUBooleanConverter))]
        public bool Visible
        {
            get { return (m_visible); }
            set { m_visible = value; }
        }
        /// <summary> Получить (установить) имя свойства </summary>
        /// <value> имя </value>
        [UserDesignAttribute(false)]
        public string PropertyName
        {
            get { return (String.IsNullOrEmpty(m_name) ? "Не задано" : m_name); }
            set { m_name = value; }
        }
        /// <summary> Получить (установить) отображаемое имя свойства (выводимое пользователю) </summary>
        /// <value> имя </value>
        [UserDesignAttribute(false)]
        public string DisplayName
        {
            get { return (String.IsNullOrEmpty(m_displayName) ? "Не задано" : m_displayName); }
            set { m_displayName = value; }
        }
        /// <summary> Получить (установить) описание свойства </summary>
        /// <value> описание свойства </value>
        [UserDesignAttribute(false)]
        public string Description
        {
            get { return (String.IsNullOrEmpty(m_description) ? "Не задано" : m_description); }
            set { m_description = value; }
        }

        #endregion
    }
}