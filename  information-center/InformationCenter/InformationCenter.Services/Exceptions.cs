using System;
using System.Collections;

namespace InformationCenter.Services
{

    public class ItemIsNullException : Exception
    {

        #region Поля

        private IList source;
        private int index;

        #endregion

        #region Конструкторы

        public ItemIsNullException(IList Collection, int Index) : base()
        {
            source = Collection;
            index = Index;
        }

        #endregion

        #region Свойства

        public IList Collection { get { return source; } }

        public int Index { get { return index; } }

        public override string Message
        {
            get { return string.Format("Элемент № {0} коллекции равен null.", Index); }
        }

        #endregion

    }

    public class FieldNotFoundException : Exception
    {

        #region Поля

        private Guid id;

        #endregion

        #region Конструкторы

        public FieldNotFoundException(Guid ID) : base()
        {
            id = ID;
        }

        #endregion

        #region Свойства

        public Guid ID { get { return id; } }

        public override string Message
        {
            get { return string.Format("Поле с идентификатором {0} не найдено в хранилище.", ID); }
        }

        #endregion

    }

    public class DotNetTypeNotExistsException : Exception
    {

        #region Поля

        private string t_name;

        #endregion

        #region Конструкторы

        public DotNetTypeNotExistsException(string TypeName) : base()
        {
            t_name = TypeName;
        }

        #endregion

        #region Свойства

        public string TypeName { get { return t_name; } }

        public override string Message
        {
            get { return string.Format("Невозможно создать объект типа поля по имени типа ({0})", TypeName); }
        }

        #endregion

    }

    public class NullableValueNotAllowedException : Exception
    {

        #region Поля

        private FieldView fv;

        #endregion

        #region Конструкторы

        public NullableValueNotAllowedException(FieldView FieldView) : base()
        {
            if (FieldView == null) throw new ArgumentNullException("FieldView");
            fv = FieldView;
        }

        #endregion

        #region Свойства

        public FieldView FieldView { get { return fv; } }

        public override string Message
        {
            get { return string.Format("Поле {0} не поддерживает значения null.", fv.Name); }
        }

        #endregion

    }

    public class TypeMismatchException : Exception
    {

        #region Поля

        private Type exp, real;

        #endregion

        #region Конструкторы

        public TypeMismatchException(Type Expected, Type Real) : base()
        {
            exp = Expected;
            real = Real;
        }

        #endregion

        #region Свойства

        public Type ExpectedType { get { return exp; } }

        public Type RealType { get { return real; } }

        public override string Message
        {
            get { return string.Format("Ошибка соответствия типов. Ожидаемый тип: {0}, реальный тип: {1}", ExpectedType, RealType); }
        }

        #endregion

    }

    public class FileSizeOverflowException : Exception
    {

        #region Поля

        private long file_len, limit;
        private string fileName;

        #endregion

        #region Конструкторы

        public FileSizeOverflowException(long FileLengthInBytes, long LimitInBytes, string FileName) : base()
        {
            file_len = FileLengthInBytes;
            limit = LimitInBytes;
            fileName = FileName;
        }

        #endregion

        #region Свойства

        public long FileLengthInBytes { get { return file_len; } }

        public long LimitInBytes { get { return limit; } }

        public string FileName { get { return fileName; } }

        public override string Message
        {
            get { return string.Format("Размер файла в {0} Кб превышает максимально допустимый размер {1} Кб.", (int)(file_len / 1024), (int)(limit / 1024)); }
        }

        #endregion

    }

    public class NotSupportedFieldTypeException : Exception
    {

        #region Поля

        private Type t;

        #endregion

        #region Конструкторы

        public NotSupportedFieldTypeException(Type Type) : base()
        {
            t = Type;
        }

        #endregion

        #region Свойства

        public Type Type { get { return t; } }

        public override string Message
        {
            get { return string.Format("Тип поля '{0}' не поддерживается."); }
        }

        #endregion

    }

}