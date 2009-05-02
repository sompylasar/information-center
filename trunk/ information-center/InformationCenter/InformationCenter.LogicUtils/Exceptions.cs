using System;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace LogicUtils
{

    /// <summary>
    /// Класс ошибки для случая, когда для некоторого типа не найден необходимый конструктор.
    /// </summary>
    public class ConstructorNotFoundException : Exception
    {

        #region Поля

        private Type[] prms = null;
        private Type t = null;
        private string t_string = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="T">требуемый тип</param>
        /// <param name="Parameters">параметры конструктора, который не был найден</param>
        public ConstructorNotFoundException(Type T, Type[] Parameters)
        {
            if (T == null) throw new ArgumentNullException("T");
            if (Parameters == null) throw new ArgumentNullException("Parameters");
            t = T;
            prms = Parameters;
            StringBuilder b = new StringBuilder("(", 100);
            foreach (Type type in prms)
            {
                if (type == null) throw new NullReferenceException("Параметр \"Parameters\" не может содержать значений null.");
                b.Append(type.FullName).Append(", ");
            }
            if (prms.Length != 0) b.Remove(b.Length - 2, 2);
            b.Append(")");
            t_string = b.ToString();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// получить параметры конструктора
        /// </summary>
        public Type[] Parameters { get { return Parameters; } }

        /// <summary>
        /// получить тип
        /// </summary>
        public Type Type { get { return t; } }

        /// <summary>
        /// получить сообщение об ошибке
        /// </summary>
        public override string Message { get { return "В типе " + t.FullName + " не найден конструктор " + t.FullName + t_string; } }

        #endregion

    }

    /// <summary>
    /// Класс ошибки для случая, когда один клас должен быть наследником другого класса.
    /// </summary>
    public class TypeInheritanceException : Exception
    {

        #region Поля

        private Type d = null, b = null;

        #endregion

        #region Конструкторы

        public TypeInheritanceException(Type Derived, Type Base)
        {
            if (Derived == null) throw new ArgumentNullException("Derived");
            if (Base == null) throw new ArgumentNullException("Base");
            d = Derived;
            b = Base;
        }

        #endregion

        #region Свойства

        public override string Message
        {
            get { return string.Format("Класс {0} должен наследоваться от класса {1}", d.FullName, b.FullName); }
        }

        #endregion

    }

    /// <summary>
    /// Класс-исключение : Отсутствие типа
    /// </summary>
    [Serializable, ComVisible(true)]
    public sealed class TypeNotFoundException : Exception
    {

        #region Поля

        /// <summary>
        /// Имя типа
        /// </summary>
        private string m_typename;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public TypeNotFoundException() : base("Отсутствует тип") { }

        /// <summary>
        /// Конструктор с сообщением
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        public TypeNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Конструктор с сообщением и именем интерфейса
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="typename"> имя типа </param>
        public TypeNotFoundException(string message, string typename)
            : base(message)
        {
            m_typename = typename;
        }

        /// <summary>
        /// Конструктор с сообщением и внутренним исключением
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="innerException"> исключение </param>
        public TypeNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Конструктор для десериализации объекта
        /// </summary>
        /// <param name="info"> объект с полной информацией </param>
        /// <param name="context"> контекст для десериализации </param>
        private TypeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_typename = info.GetString("TypeName");
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получить сообщение об ошибке
        /// </summary>
        /// <value> текст ошибки </value>
        public override string Message
        {
            get
            {
                if (m_typename == null)
                    return (base.Message);
                StringBuilder sb = new StringBuilder(base.Message);
                sb.AppendFormat(Environment.NewLine + "Имя типа : {0}", m_typename);
                return (sb.ToString());
            }
        }

        /// <summary>
        /// Получить полное имя типа
        /// </summary>
        /// <value> имя типа </value>
        public string TypeName
        {
            get { return (m_typename); }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для сериализации объекта
        /// </summary>
        /// <param name="info"> объект, содежащий данные сериализации </param>
        /// <param name="context"> контекст для сериализации </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("TypeName", m_typename, typeof(string));
        }

        #endregion
    }

    /// <summary>
    /// Класс-исключение : Отсутствие интерфейса
    /// </summary>
    [Serializable, ComVisible(true)]
    public sealed class InterfaceNotFoundException : Exception
    {

        #region Поля

        /// <summary>
        /// Имя интерфейса
        /// </summary>
        private string m_interfacename;

        /// <summary>
        /// Имя типа, у которого отсутствует интерфейс
        /// </summary>
        private string m_typename;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public InterfaceNotFoundException() : base("Отсутствует интерфейс") { }

        /// <summary>
        /// Конструктор с сообщением
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        public InterfaceNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Конструктор с сообщением и именем интерфейса
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="interfacename"> имя интерфейса </param>
        public InterfaceNotFoundException(string message, string interfacename)
            : base(message)
        {
            m_interfacename = interfacename;
        }

        /// <summary>
        /// Конструктор с сообщением, именем интерфейса и типом объекта
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="interfacename"> имя интерфейса </param>
        /// <param name="type"> тип объекта </param>
        public InterfaceNotFoundException(string message, string interfacename, Type type)
            : this(message, interfacename)
        {
            if (type != null)
                m_typename = type.FullName;
        }

        /// <summary>
        /// Конструктор с сообщением и внутренним исключением
        /// </summary>
        /// <param name="message"> текст сообщения </param>
        /// <param name="innerException"> исключение </param>
        public InterfaceNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Конструктор для десериализации объекта
        /// </summary>
        /// <param name="info"> объект с полной информацией </param>
        /// <param name="context"> контекст для десериализации </param>
        private InterfaceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_interfacename = info.GetString("InterfaceName");
            m_typename = info.GetString("TypeName");
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получить сообщение об ошибке
        /// </summary>
        /// <value> текст ошибки </value>
        public override string Message
        {
            get
            {
                if (m_interfacename == null && m_typename == null)
                    return (base.Message);
                StringBuilder sb = new StringBuilder(base.Message);
                if ((m_interfacename != null) && (m_interfacename.Length != 0))
                {
                    sb.AppendFormat(Environment.NewLine + "Интерфейс : {0}", m_interfacename);
                }
                if ((m_typename != null) && (m_typename.Length != 0))
                {
                    sb.AppendFormat(Environment.NewLine + "Имя типа : {0}", m_typename);
                }
                return (sb.ToString());
            }
        }

        /// <summary>
        /// Получить полное имя типа, у которого отсутствует интерфейс
        /// </summary>
        /// <value> имя типа </value>
        public string TypeName
        {
            get { return (m_typename); }
        }

        /// <summary>
        /// Получить имя интерфейса, вызвавшего ошибку
        /// </summary>
        public string InterfaceName
        {
            get { return (m_interfacename); }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод для сериализации объекта
        /// </summary>
        /// <param name="info"> объект, содежащий данные сериализации </param>
        /// <param name="context"> контекст для сериализации </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("InterfaceName", m_interfacename, typeof(string));
            info.AddValue("TypeName", m_typename, typeof(string));
        }

        #endregion
    }

    /// <summary>
    /// Класс исключения по отсутствию в классе заданного свойства.
    /// </summary>
    public class PropertyBaseException : Exception
    {

        #region Поля

        /// <summary>
        /// имя свойства
        /// </summary>
        protected string prop_name;

        /// <summary>
        /// имя класса, в котором находится свойство
        /// </summary>
        protected string class_name;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="PropertyName">имя свойства</param>
        /// <param name="ClassName">имя класса</param>
        public PropertyBaseException(string PropertyName, string ClassName)
        {
            prop_name = PropertyName;
            class_name = ClassName;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// получить имя свойства
        /// </summary>
        public string PropertyName { get { return prop_name; } }

        /// <summary>
        /// получить имя класса, у которого не найдено свойство 
        /// </summary>
        public string ClassName { get { return class_name; } }

        /// <summary>
        /// получить сообщение об ошибке
        /// </summary>
        public override string Message
        { get { return "Ошибка в классе " + class_name + " в свойстве " + prop_name + "."; } }

        #endregion

    }

    /// <summary>
    /// Класс исключения по отсутствию в классе заданного свойства.
    /// </summary>
    public class PropertyNotFoundException : PropertyBaseException
    {

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="PropertyName">имя свойства</param>
        /// <param name="ClassName">имя класса</param>
        public PropertyNotFoundException(string PropertyName, string ClassName) : base(PropertyName, ClassName) { }

        #endregion

        #region Свойства

        /// <summary>
        /// получить сообщение об ошибке
        /// </summary>
        public override string Message
        { get { return "В классе " + class_name + " не найдено свойство " + prop_name + "."; } }

        #endregion

    }

    /// <summary>
    /// Класс исключения для свойства, не поддерживающего чтение.
    /// </summary>
    public class PropertyNotReadSupportedException : PropertyBaseException
    {

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="PropertyName">имя свойства</param>
        /// <param name="ClassName">имя класса</param>
        public PropertyNotReadSupportedException(string PropertyName, string ClassName) : base(PropertyName, ClassName) { }

        #endregion

        #region Свойства

        /// <summary>
        /// получить сообщение об ошибке
        /// </summary>
        public override string Message
        { get { return "Свойство " + prop_name + " класса " + class_name + " недоступно для чтения."; } }

        #endregion

    }

    /// <summary>
    /// Класс исключения для свойства, не поддерживающего запись.
    /// </summary>
    public class PropertyNotWriteSupportedException : PropertyBaseException
    {

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="PropertyName">имя свойства</param>
        /// <param name="ClassName">имя класса</param>
        public PropertyNotWriteSupportedException(string PropertyName, string ClassName) : base(PropertyName, ClassName) { }

        #endregion

        #region Свойства

        /// <summary>
        /// получить сообщение об ошибке
        /// </summary>
        public override string Message
        { get { return "Свойство " + prop_name + " класса " + class_name + " недоступно для записи."; } }

        #endregion

    }

    /// <summary>
    /// Базовый класс для исключения, использующего Enum.
    /// </summary>
    public abstract class EnumException : Exception
    {

        #region Свойства

        /// <summary>
        /// свойство, работающее с конкретным типом перечисления в производных классах
        /// </summary>
        public abstract Enum Value { get; set; }

        /// <summary>
        /// текст ошибки (выводятся все описания ошибок, определённые в Value)
        /// </summary>
        public override string Message
        { get { return EnumFieldDescriptionConverter.GetEnumFlagsDescriptionString(Value); } }

        #endregion

    }

    /// <summary>
    /// Исключение возникает когда переменной присвоено недопустимое значение
    /// </summary>
    [global::System.Serializable]
    public class NonAcceptableValueException : Exception
    {
        public NonAcceptableValueException() { }
        public NonAcceptableValueException(string message) : base(message) { }
        public NonAcceptableValueException(string message, Exception inner) : base(message, inner) { }
        protected NonAcceptableValueException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Исключение возникает, когда объект, имеющий состояние инициализирован/не_инициализирован,
    /// используется в не инициализированном состоянии
    /// </summary>
    [global::System.Serializable]
    public class NonInitializedException : Exception
    {
        public NonInitializedException() { }
        public NonInitializedException(string message) : base(message) { }
        public NonInitializedException(string message, Exception inner) : base(message, inner) { }
        protected NonInitializedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Исключение возникает при попытке создания нового объекта в хранилище
    /// </summary>
    [global::System.Serializable]
    public class EntityCreateException : Exception
    {
        public EntityCreateException() { }
        public EntityCreateException(string message) : base(message) { }
        public EntityCreateException(string entityName, string message)
            : base(message)
        {
            EntityName = entityName;
        }
        public EntityCreateException(string message, Exception inner) : base(message, inner) { }
        public EntityCreateException(string entityName, string message, Exception inner)
            : base(message, inner)
        {
            EntityName = entityName;
        }
        protected EntityCreateException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }

        public string EntityName { get; private set; }
    }

    /// <summary>
    /// Исключение при ORM
    /// </summary>
    /*[global::System.Serializable]
    public class MappingException : Exception
    {
        public MappingException() { }
        public MappingException(string message) : base(message) { }
        public MappingException(string message, Exception inner) : base(message, inner) { }
        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }*/

}