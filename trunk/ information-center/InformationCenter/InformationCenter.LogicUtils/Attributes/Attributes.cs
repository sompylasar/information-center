using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Drawing;

namespace LogicUtils.Attributes
{
    /// <summary> Абстрактный переопределяемый (перезаписываемый) атрибут </summary>
    /// <remarks> Значение данного атрибута может меняться в процессе работы </remarks>
    public abstract class OverridableAttribute : Attribute { }
    /// <summary> Коллекция переопределяемых (перезаписываемых) атрибутов </summary>
    [Description("Коллекция переопределяемых (перезаписываемых) атрибутов")]
    public sealed class OverridableAttributeCollection : System.Collections.ObjectModel.Collection<OverridableAttribute>
    {
        /// <summary> Конструктор с атрибутами </summary>
        /// <param name="attributes"> атрибуты </param>
        public OverridableAttributeCollection(IEnumerable attributes) : base()
        {
            if (attributes != null)
            {
                IEnumerator enumerator = attributes.GetEnumerator();
                try
                {
                    OverridableAttribute overAttribute;
                    while (enumerator.MoveNext())
                    {
                        overAttribute = enumerator.Current as OverridableAttribute;
                        if (overAttribute != null)
                            base.Add(overAttribute);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
        }
        /// <summary> Получить атрибут по типу </summary>
        /// <param name="attributeType"> тип атрибута </param>
        /// <returns> атрибут или null </returns>
        public OverridableAttribute this[System.Type type]
        {
            get
            {
                for (int i = 0; i < base.Count; i++)
                    if (base[i].GetType() == type)
                        return (base[i]);

                return (null);
            }
        }
        /// <summary> Получить атрибуты с изменеными значениями </summary>
        /// <param name="componentType"> тип, для которого определяются атрибуты с измененными значениями </param>
        /// <param name="propertyName"> имя свойства </param>
        /// <returns> массив атрибутов </returns>
        /// <remarks> Свойство типа должно быть помечено атрибутом BindableAttribute.Yes (т.е. должно быть связываемым) </remarks>
        public OverridableAttribute[] GetDifferences(System.Type componentType, string propertyName)
        {
            // выходной список атрибутов с измененными значениями
            List<OverridableAttribute> listResult = new List<OverridableAttribute>();
            // все свойства типа
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(componentType, new Attribute[] { BindableAttribute.Yes });
            // наше свойство
            PropertyDescriptor property = properties[propertyName];
            if (property != null)
            {
                AttributeCollection defaultAttributeCollection = property.Attributes;
                for (int i = 0; i < base.Count; i++)
                {
                    if (!defaultAttributeCollection.Matches(base[i]))
                        listResult.Add(base[i]);
                }
            }
            // возвращаем массив
            return (listResult.ToArray());
        }
    }
    /// <summary> Атрибут видимости свойства </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class VisibleAttribute : OverridableAttribute
    {
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly VisibleAttribute Default = new VisibleAttribute(false);
        /// <summary> Атрибут с установленным значением </summary>
        public static readonly VisibleAttribute Yes = new VisibleAttribute(true);
        /// <summary> Атрибут с не установленным значением </summary>
        public static readonly VisibleAttribute No = new VisibleAttribute(false);
        /// <summary> Флаг видимости (отображения) </summary>
        private bool m_visible;
        /// <summary> Конструктор с признаком видимости </summary>
        /// <param name="isVisible"> признак видимости </param>
        public VisibleAttribute(bool isVisible) { m_visible = isVisible; }
        /// <summary> Получить признак видимости </summary>
        /// <value> true, если свойство отображается, в противном случае - false. </value>
        public bool Visible { get { return (m_visible); } }
        /// <summary> Проверка на равенство </summary>
        /// <param name="obj"> сравниваемый объект </param>
        /// <returns> true если атрибуты имеют одинаковое значение, в противном случае - false </returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return (true);

            VisibleAttribute attribute = obj as VisibleAttribute;
            return ((attribute != null) && (attribute.Visible == m_visible));
        }
        /// <summary> Получить хэш-код </summary>
        /// <returns> 32-bit знаковое челое число хэш кода </returns>
        public override int GetHashCode() { return (typeof(VisibleAttribute).GetHashCode() ^ (m_visible ? -1 : 0)); }
        /// <summary> Проверка на знчение атрибута по-умолчанию </summary>
        /// <returns> true если значение атрибута по-умолчанию, в противном случае - false. </returns>
        public override bool IsDefaultAttribute() { return (this.Visible == Default.Visible); }
    }
    /// <summary> Атрибут горизонтального выравнивания текста </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class HorizontalTextAlignmentAttribute : OverridableAttribute
    {
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly HorizontalTextAlignmentAttribute Default = new HorizontalTextAlignmentAttribute(ContentAlignment.MiddleLeft);
        /// <summary> Выравнивание </summary>
        private readonly ContentAlignment m_alignment;
        /// <summary> Конструктор с типом выравнивания </summary>
        /// <param name="alignment"> выравнивание </param>
        public HorizontalTextAlignmentAttribute(ContentAlignment alignment) { m_alignment = alignment; }
        /// <summary> Получить тип выравнивания </summary>
        /// <value> тип выравнивания </value>
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public ContentAlignment Alignment { get { return (m_alignment); } }
        /// <summary> Проверка на знчение атрибута по-умолчанию </summary>
        /// <returns> true если значение атрибута по-умолчанию, в противном случае - false. </returns>
        public override bool IsDefaultAttribute() { return (this.Equals(Default)); }
        /// <summary> Проверка на равенство </summary>
        /// <param name="obj"> сравниваемый объект </param>
        /// <returns> true если атрибуты имеют одинаковое значение, в противном случае - false </returns>
        public override bool Equals(object obj)
        {
            if (obj == this) return (true);

            HorizontalTextAlignmentAttribute attribute = obj as HorizontalTextAlignmentAttribute;
            return ((attribute != null) && (attribute.Alignment == m_alignment));
        }
        /// <summary> Получить хэш-код </summary>
        /// <returns> 32-bit знаковое челое число хэш кода </returns>
        public override int GetHashCode() { return (m_alignment.GetHashCode()); }
    }
    /// <summary> Атрибут порядка отображения </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class DisplayIndexAttribute : OverridableAttribute
    {
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly DisplayIndexAttribute Default = new DisplayIndexAttribute(Int32.MaxValue);
        /// <summary> Индекс (порядковый номер) </summary>
        private readonly int m_index;
        /// <summary> Конструктор с индексом </summary>
        /// <param name="index"> индекс </param>
        public DisplayIndexAttribute(int index) 
        {
            m_index = (index >= 0)? index : Default.Index;
        }
        /// <summary> Получить индекс </summary>
        /// <value> индекс </value>
        public int Index { get { return (m_index); } }
    }
    /// <summary> Атрибут относительной ширины (колонки) </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class FillWeightAttribute : OverridableAttribute
    {
        /// <summary> Максимальное значение </summary>
        private const float MAX_VALUE = 100;
        /// <summary> Минимальное значение </summary>
        private const float MIN_VALUE = 0;
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly FillWeightAttribute Default = new FillWeightAttribute(MAX_VALUE);
        /// <summary> Относительная ширина </summary>
        private readonly float m_fillWeight;
        /// <summary> Конструктор </summary>
        /// <param name="weight"> относительная ширина </param>
        public FillWeightAttribute(float weight)
        {
            m_fillWeight = ((weight <= MIN_VALUE) || (weight > MAX_VALUE)) ? MAX_VALUE : weight;
        }
        /// <summary> Получить относительную ширину </summary>
        /// <value> ширина </value>
        public float Weight { get { return (m_fillWeight); } }
    }
    /// <summary> Атрибут режима вывода (колонки) </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class AutoSizeModeAttribute : OverridableAttribute
    {
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly AutoSizeModeAttribute Default = new AutoSizeModeAttribute(AutoSizeModes.NotSet);
        /// <summary> Режим автоширины колонок </summary>
        private readonly AutoSizeModes m_mode;
        /// <summary> Конструктор с режимом автоширины колонок</summary>
        /// <param name="mode"> режим </param>
        public AutoSizeModeAttribute(AutoSizeModes mode) { m_mode = mode; }
        /// <summary> Получить режим автоширины колонок </summary>
        /// <value> режим </value>
        public AutoSizeModes AutoSizeMode { get { return (m_mode); } }
    }
    /// <summary> Виды автоширины колонок таблицы </summary>
    [Description("Режим автоширины колонок таблицы")]
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum AutoSizeModes
    {
        /// <summary> не установлено </summary>
        [Description("не установлено")]
        NotSet = 0,
        /// <summary> нет автоширины </summary>
        [Description("нет автоширины")]
        None = 1,
        /// <summary> по заголовку </summary>
        [Description("по заголовку")]
        ColumnHeader = 2,
        /// <summary> все ячейки, не включая заголовок </summary>
        [Description("по всем ячейкам (кроме заголовка)")]
        AllCellsExceptHeader = 4,
        /// <summary> все ячейки, включая заголовок </summary>
        [Description("по всем ячейкам")]
        AllCells = 6,
        /// <summary> все видимые ячейки, не включая заголовок </summary>
        [Description("по всем видимым ячейкам (кроме заголовка)")]
        DisplayedCellsExceptHeader = 8,
        /// <summary> все видимые ячейки, включая заголовок </summary>
        [Description("по всем видимым ячейкам")]
        DisplayedCells = 10,
        /// <summary> по всей ширине таблицы </summary>
        [Description("по всей ширине таблицы")]
        Fill = 0x10
    }
    /// <summary> Атрибут пользовательской настройки </summary>
    /// <remarks> Свойство, помеченное данным атрибутом, может настраиваться пользователем </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    [Description("Атрибут пользовательской настройки")]
    public sealed class UserDesignAttribute : Attribute
    {
        /// <summary> Атрибут по-молчанию </summary>
        public static readonly UserDesignAttribute Default = new UserDesignAttribute(false);
        /// <summary> Атрибут с установленным значением </summary>
        public static readonly UserDesignAttribute Yes = new UserDesignAttribute(true);
        /// <summary> Атрибут с не установленным значением </summary>
        public static readonly UserDesignAttribute No = new UserDesignAttribute(false);
        /// <summary> Флаг настройки </summary>
        private bool m_designed;
        /// <summary> Конструктор с признаком пользовательской настройки </summary>
        /// <param name="designed"> признак пользовательской настройки </param>
        public UserDesignAttribute(bool designed) { m_designed = designed; }
        /// <summary> Получить признак пользовательской настройки </summary>
        /// <value> true, если пользовательская настройка возможна, в противном случае - false. </value>
        public bool Designed { get { return (m_designed); } }
        /// <summary> Проверка на равенство </summary>
        /// <param name="obj"> сравниваемый объект </param>
        /// <returns> true если атрибуты имеют одинаковое значение, в противном случае - false </returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return (true);

            UserDesignAttribute attribute = obj as UserDesignAttribute;
            return ((attribute != null) && (attribute.Designed == m_designed));
        }
        /// <summary> Получить хэш-код </summary>
        /// <returns> 32-bit знаковое челое число хэш кода </returns>
        public override int GetHashCode() { return (typeof(UserDesignAttribute).GetHashCode() ^ (m_designed ? -1 : 0)); }
        /// <summary> Проверка на знчение атрибута по-умолчанию </summary>
        /// <returns> true если значение атрибута по-умолчанию, в противном случае - false. </returns>
        public override bool IsDefaultAttribute() { return (this.Designed == Default.Designed); }
    }
}
