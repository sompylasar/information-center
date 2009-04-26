using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LogicUtils.Attributes
{
    public class TunableTypeDescriptor:CustomTypeDescriptor
    {
        public TunableTypeDescriptor(ICustomTypeDescriptor parent) :
            base(parent) { }

        #region ICustomTypeDescriptor
        /// <summary> Получить коллекцию свойств типа </summary>
        /// <returns> сортированная коллекция свойств типа </returns>
        public override PropertyDescriptorCollection GetProperties() { return (Convert(base.GetProperties(), null)); }
        /// <summary> Получить коллекцию свойств типа с указанными атрибутами </summary>
        /// <param name="attributes"> атрибуты </param>
        /// <returns> сортированная коллекция свойств типа </returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return (Convert(base.GetProperties(attributes), attributes)); }

        #endregion

        #region Внутренние методы

        /// <summary> Преобразование типа коллекции </summary>
        /// <param name="properties"> коллекция свойств типа </param>
        /// <param name="attributes"> атрибуты </param>
        /// <returns> коллекция свойств типа </returns>
        /// <remarks> Делает фильтрацию по атрибутам, потому что базовый класс их не понимает </remarks>
        private static BindTunablePropertyDescriptorCollection Convert(PropertyDescriptorCollection properties, Attribute[] attributes)
        {
            return (new BindTunablePropertyDescriptorCollection(FilterSortMembers(properties, attributes).ToArray()));
        }
        /// <summary> Фильтрация свойств по атрибутам </summary>
        /// <param name="members"> список свойств </param>
        /// <param name="attributes"> атрибуты </param>
        /// <returns> сортированный список свойств </returns>
        private static List<BindTunablePropertyDescriptor> FilterSortMembers(PropertyDescriptorCollection members, Attribute[] attributes)
        {
            int count = members.Count;
            List<BindTunablePropertyDescriptor> list = new List<BindTunablePropertyDescriptor>(count);
            for (int i = 0; i < count; i++)
            {
                if (!ShouldHideMember((MemberDescriptor)members[i], attributes))
                    list.Add(new BindTunablePropertyDescriptor(members[i]));
            }
            // сортируем по индексу
            list.Sort(delegate(BindTunablePropertyDescriptor x, BindTunablePropertyDescriptor y) { return (x.Index - y.Index); });
            return (list);

            //int count = members.Count;
            //List<BindTunablePropertyDescriptor> list = new List<BindTunablePropertyDescriptor>(count);
            //for (int i = 0; i < count; i++)
            //{
            //    BindableAttribute ba = members[i].Attributes[typeof(BindableAttribute)] as BindableAttribute;
            //    bool bindable = (ba != null) && ba.Bindable;
            //    if (bindable && (!ShouldHideMember((MemberDescriptor)members[i], attributes)))
            //        list.Add(new BindTunablePropertyDescriptor(members[i]));
            //}
            //// сортируем по индексу
            //list.Sort(delegate(BindTunablePropertyDescriptor x, BindTunablePropertyDescriptor y) { return (x.Index - y.Index); });
            //return (list);
        }
        /// <summary> Проверка на недопустимое свойство </summary>
        /// <param name="member"> свойство </param>
        /// <param name="attributes"> массив атрибутов </param>
        /// <returns> true если свойство не описано с указанным атрибутом, в противном случае - false </returns>
        private static bool ShouldHideMember(MemberDescriptor member, Attribute[] attributes)
        {
            if (member == null)
                return (true);
            if (!IsValidArray(attributes))
                return (false);
            
            for (int i = 0; i < attributes.Length; i++)
            {
                Attribute attribute2 = member.Attributes[attributes[i].GetType()];
                if (((attribute2 == null) && (!attributes[i].IsDefaultAttribute())) || !attributes[i].Match(attribute2))
                    return (true);
            }
            return (false);
        }
        /// <summary> Проверка массива на валидность </summary>
        /// <param name="ary"> массив </param>
        /// <returns> true если массив валиден, в противном случае - false </returns>
        private static bool IsValidArray(Array ary)
        {
            return ((ary != null) && (ary.Length > 0));
        }

        #endregion
    }
    /// <summary> Коллекция настраиваемых связываемых свойств объектов данных </summary>
    public class BindTunablePropertyDescriptorCollection : PropertyDescriptorCollection, IEnumerable<BindTunablePropertyDescriptor>
    {
        /// <summary> Конструктор с массивом настраиваемых связываемых свойств </summary>
        /// <param name="properties"> массив свойств </param>
        public BindTunablePropertyDescriptorCollection(BindTunablePropertyDescriptor[] properties) : base(properties) { }
        /// <summary> Получить описатель свойства по индексу </summary>
        /// <param name="index"> индекс </param>
        /// <returns> описталь свойства типа BindTunablePropertyDescriptor </returns>
        public new BindTunablePropertyDescriptor this[int index] { get { return ((BindTunablePropertyDescriptor)base[index]); } }
        /// <summary> Получить описатель свойства по имени </summary>
        /// <param name="name"> имя </param>
        /// <returns> описталь свойства типа BindTunablePropertyDescriptor </returns>
        public new BindTunablePropertyDescriptor this[string name] { get { return ((BindTunablePropertyDescriptor)base[name]); } }
        /// <summary> Получить итератор </summary>
        /// <returns> итератор </returns>
        /// <remarks> Итератор по объектам BindTunablePropertyDescriptor </remarks>
        IEnumerator<BindTunablePropertyDescriptor> IEnumerable<BindTunablePropertyDescriptor>.GetEnumerator() { return (this.FirstToLastT.GetEnumerator()); }
        /// <summary> Итератор по объектам BindTunablePropertyDescriptor </summary>
        /// <value> прямой итератор </value>
        private IEnumerable<BindTunablePropertyDescriptor> FirstToLastT
        {
            get
            {
                for (int i = 0; i < base.Count; i++)
                    yield return this[i];
            }
        }
    }
    /// <summary> Провайдер описателей свойств для объектов данных модели </summary>
    public sealed class TunableTypeDescriptionProvider<T> : TypeDescriptionProvider
    {
        private TunableTypeDescriptor descriptor;

        #region Конструкторы

        /// <summary> Конструктор по-умолчанию </summary>
        public TunableTypeDescriptionProvider() : this(TypeDescriptor.GetProvider(typeof(T))) { }
        /// <summary> Конструктор с родительским провайдером </summary>
        /// <param name="parent"> родительский провайдер </param>
        public TunableTypeDescriptionProvider(TypeDescriptionProvider parent) : base(parent) { }

        #endregion

        /// <summary> Получить описатель свойств объекта </summary>
        /// <param name="objectType"> тип объекта </param>
        /// <param name="instance"> объект </param>
        /// <returns> Описатель свойств объекта </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            if (descriptor == null)
                descriptor = new TunableTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
            return descriptor;
        }
    }
}
