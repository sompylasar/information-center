using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Globalization;

namespace LogicUtils
{

    /// <summary>
    /// Класс пары имя - приоритет.
    /// </summary>
    public class PropertyOrderPair : IComparable
    {

        #region Поля

        private int _order;
        private string _name;

        #endregion

        #region Конструкторы

        /// <summary>
        /// основной конструктор
        /// </summary>
        /// <param name="Name">имя объекта</param>
        /// <param name="Order">приоритет</param>
        public PropertyOrderPair(string Name, int Order) { _order = Order; _name = Name; }

        #endregion

        #region Свойства

        /// <summary>
        /// получить имя
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// получить приоритет
        /// </summary>
        public int Order { get { return _order; } }

        #endregion

        #region Методы

        /// <summary>
        /// собственно метод сравнения
        /// </summary>
        public int CompareTo(object obj)
        {
            int otherOrder = ((PropertyOrderPair)obj)._order;
            if (otherOrder == _order) // если Order одинаковый - сортируем по именам
                return string.Compare(_name, ((PropertyOrderPair)obj)._name);
            else if (otherOrder > _order) return -1;
            return 1;
        }

        #endregion

    }

    /// <summary>
    /// Класс конвертирует значение энума в список строк-описателей проставленных в нём полей.
    /// Строки-описатели должны задаваться с помощью [DescriptionAttribute()].
    /// </summary>
    public static class EnumFieldDescriptionConverter
    {

        /// <summary>
        /// конвертация значений энума
        /// </summary>
        /// <param name="EnumValue">энум</param>
        /// <returns>список строк</returns>
        public static string[] GetEnumFlagsDescriptionStringArray(Enum EnumValue)
        {
            if (EnumValue == null) return null;
            Type t = EnumValue.GetType();
            object[] attrs = t.GetCustomAttributes(typeof(FlagsAttribute), false);
            if (attrs.Length < 1) return null;

            List<string> result = new List<string>();
            Array values = Enum.GetValues(t);
            int IntRepresebtation = EnumToIntConverter.Convert(EnumValue);
            for (int i = 0; i < values.GetLength(0); ++i)
                if (((int)values.GetValue(i) & IntRepresebtation) != 0)
                    result.Add(EnumDescriptionConverter.GetEnumDescription((Enum)values.GetValue(i)));
            return result.ToArray();
        }

        /// <summary>
        /// конвертация значений энума
        /// </summary>
        /// <param name="EnumValue">энум</param>
        /// <returns>одна строка, где все подстроки расположены каждая на своей строке (\n)</returns>
        public static string GetEnumFlagsDescriptionString(Enum EnumValue)
        {
            string[] result = EnumFieldDescriptionConverter.GetEnumFlagsDescriptionStringArray(EnumValue);
            string output = string.Empty;
            for (int i = 0; i < result.Length; ++i)
                output += (result[i] + "\n");
            return output;
        }
    }

    /// <summary> Конвертор для перечислений </summary>
    /// <remarks> C битовыми флагами не работает </remarks>
    public sealed class EnumDescriptionConverter : EnumConverter
    {
        /// <summary> Наш тип (тип перечисления) </summary>
        private System.Type m_enumtype;
        /// <summary> Конструктор с типом </summary>
        /// <param name="type"> тип перечисления </param>
        public EnumDescriptionConverter(System.Type type) : base(type) { m_enumtype = type; }
        /// <summary>
        /// Получить описание(Description) по значению элемента перечисления
        /// </summary>
        /// <param name="value"> значение </param>
        /// <returns> описание элемента (Description) </returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, (typeof(DescriptionAttribute)));
            return ((attribute != null) ? attribute.Description : value.ToString());
        }
        /// <summary>
        /// Получить описание(Description) по текстовому значению элемента перечисления
        /// </summary>
        /// <param name="type"> тип перечисления </param>
        /// <param name="value"> текстовое значение </param>
        /// <returns> описание элемента (Description) </returns>
        /// <remarks> Метод используется для получения описания (description), когда значение перечисления имеет тип string, а не Enum </remarks>
        internal static string GetEnumDescription(System.Type type, string value)
        {
            FieldInfo fi = type.GetField(value);
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, (typeof(DescriptionAttribute)));
            return ((attribute != null) ? attribute.Description : value);
        }
        /// <summary>
        /// Получить значение по описанию (Description)
        /// </summary>
        /// <param name="type"> тип перечисления </param>
        /// <param name="description"> описание </param>
        /// <returns> знчение </returns>
        internal static object GetEnumValue(System.Type type, string description)
        {
            foreach (FieldInfo fi in type.GetFields())
            {
                DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, (typeof(DescriptionAttribute)));
                if ((attribute != null) && (attribute.Description == description))
                    return (fi.GetValue(fi.Name));
            }
            return description;
        }
        /// <summary>
        /// Получить коллекцию значений
        /// </summary>
        /// <param name="context"> контекст </param>
        /// <returns> коллекция значений </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return (new StandardValuesCollection(Enum.GetValues(m_enumtype)));
        }
        /// <summary>
        /// Преобоазование в указанный тип
        /// </summary>
        /// <param name="context"> контекст </param>
        /// <param name="culture"> локаль </param>
        /// <param name="value"> значение </param>
        /// <param name="destinationType"> тип результата </param>
        /// <returns> преобразованное значение </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((value is Enum) && (destinationType == typeof(string))) // вроде должен быть только этот if
                // для строки возвращаем описание (description)
                return GetEnumDescription((Enum)value);
            if ((value is string) && (destinationType == typeof(string)))   // но иногда бывает и этот (если кто-то лоханулся)
                return (GetEnumDescription(m_enumtype, (string)value));

            return (base.ConvertTo(context, culture, value, destinationType));  // этот скорее всего выдаст ошибку
        }
        /// <summary>
        /// Преобразование из указанного типа
        /// </summary>
        /// <param name="context"> контекст </param>
        /// <param name="culture"> локаль </param>
        /// <param name="value"> значение </param>
        /// <returns> преобразованное значение </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return (GetEnumValue(m_enumtype, (string)value));
            else if (value is Enum)
                return (GetEnumDescription((Enum)value));
            else
                return (base.ConvertFrom(context, culture, value));
        }
    }

    /// <summary>
    /// Класс конвертирует Enum в int32, Enum по сути должен быть флаговым (FlagsAttribute()), но
    /// это не обязательно.
    /// </summary>
    internal static class EnumToIntConverter
    {

        /// <summary>
        /// если в энуме E проставлено поле X, то это поле будет занесено в список
        /// </summary>
        /// <param name="E">перечисление</param>
        /// <returns>список полей энума, которые в нём выставлены</returns>
        public static List<string> Vals(Enum E)
        {
            List<string> result = new List<string>();
            result.AddRange(E.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            return result;
        }

        /// <summary>
        /// конвертирует перечисление в интовое число
        /// </summary>
        /// <param name="E">перечисление</param>
        /// <returns>int32</returns>
        public static int Convert(Enum E)
        {
            int result = 0;
            List<string> fields = Vals(E);
            List<string> names_lst = new List<string>();
            List<int> vals = new List<int>();
            names_lst.AddRange(Enum.GetNames(E.GetType()));
            Array v = Enum.GetValues(E.GetType());
            for (int i = 0; i < v.GetLength(0); ++i)
                vals.Add((int)v.GetValue(i));
            for (int i = 0; i < names_lst.Count; ++i)
            {
                if (fields.Contains(names_lst[i]))
                    result |= vals[i];
            }
            return result;
        }
    }

    /// <summary>
    /// Интерфейс для совмещения функциональности конвертеров в PropertyGrid.  
    /// </summary>
    public interface IConverterFilter
    {
        PropertyDescriptorCollection GetFilteredProperties(ITypeDescriptorContext context,
            object value, Attribute[] attributes, PropertyDescriptorCollection FilteredProperties);
    }

    /// <summary>
    /// Перечислитель для композиции из двух коллекций.
    /// </summary>
    /// <typeparam name="T">тип элементов коллекции</typeparam>
    public class PairVirtualEnumerator<T> : IEnumerator<T>
    {
        
        #region Поля

        private bool use_first = true;
        private IEnumerator<T> c1, c2;

        #endregion

        #region Конструкторы

        /// <summary>
        /// основной конструктор
        /// </summary>
        /// <param name="FirstItem">первая коллекция</param>
        /// <param name="SecondItem">вторая коллекция</param>
        public PairVirtualEnumerator(IEnumerable<T> FirstItem, IEnumerable<T> SecondItem)
        {
            c1 = FirstItem.GetEnumerator();
            c2 = SecondItem.GetEnumerator();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// получить текущий элемент
        /// </summary>
        public T Current { get { return use_first ? c1.Current : c2.Current; } } 

        /// <summary>
        /// получить текущий элемент
        /// </summary>
        object IEnumerator.Current { get { return use_first ? c1.Current : c2.Current; } }

        #endregion

        #region Методы

        /// <summary>
        /// освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            c1.Dispose();
            c2.Dispose();
        }

        /// <summary>
        /// передвинуть указатель перечислителя на следующий элемент
        /// </summary>
        /// <returns>true, если передвижение возможно</returns>
        public bool MoveNext()
        {
            if (c1.MoveNext()) { use_first = true; return true; }
            else { use_first = false; return c2.MoveNext(); }
        }

        /// <summary>
        /// сбросить перечислитель в начальное состояние
        /// </summary>
        public void Reset()
        {
            c1.Reset();
            c2.Reset();
        }

        #endregion

    }
    /// <summary> Преобразователь для типа bool </summary>
    /// <remarks> Для лузеров, которые не понимают true и false </remarks>
    [Description("Преобразователь для типа bool")]
    public sealed class RUBooleanConverter : BooleanConverter
    {
        /// <summary> Русский эквивалент значения true </summary>
        private const string RU_TRUE = "Да";
        /// <summary> Русский эквивалент значения false </summary>
        private const string RU_FALSE = "Нет";
        /// <summary> Преобоазование в указанный тип </summary>
        /// <param name="context"> контекст </param>
        /// <param name="culture"> локаль </param>
        /// <param name="value"> значение </param>
        /// <param name="destinationType"> тип результата </param>
        /// <returns> преобразованное значение </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if ((value is bool) && (destinationType == typeof(string)) && (culture.Name == "ru-RU"))
                return ((bool)value ? RU_TRUE : RU_FALSE);

            return (base.ConvertTo(context, culture, value, destinationType));
        }
        /// <summary> Преобразование из указанного типа </summary>
        /// <param name="context"> контекст </param>
        /// <param name="culture"> локаль </param>
        /// <param name="value"> значение </param>
        /// <returns> преобразованное значение </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if ((value is string) && (culture.Name == "ru-RU"))
                return (((string)value == RU_TRUE) ? true : false);

            return (base.ConvertFrom(context, culture, value));
        }
    }

}