using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data.Common;

namespace LogicUtils
{

    public delegate void DbDataReaderDelegate(DbDataReader Reader);

    /// <summary>
    /// Тип периодичности.
    /// </summary>
    public enum GroupDateEnum
    {

        /// <summary>
        /// годовая
        /// </summary>
        Year,

        /// <summary>
        /// месячная
        /// </summary>
        Month,

        /// <summary>
        /// ежедневная
        /// </summary>
        Day,

        /// <summary>
        /// без группировки
        /// </summary>
        No

    }

    /// <summary>
    /// Утилитные методы.
    /// </summary>
    public static class STD
    {

        #region Методы

        /// <summary>
        /// получить строку перехода на новую строку (аналогия std::endl в С++)
        /// </summary>
        public static string Endl { get { return System.Environment.NewLine; } }

        /// <summary>
        /// проверка равенства двух байтовых массивов
        /// </summary>
        /// <param name="first">первый байтовый массив</param>
        /// <param name="second">второй байтовый массив</param>
        /// <returns>true, если эквивалентны</returns>
        public static bool IsByteArraysEqual(byte[] first, byte[] second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("входные параметры не должны быть нулами");
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; ++i)
                if (first[i] != second[i]) return false;
            return true;
        }

        /// <summary>
        /// сравнивание двух байтовых массивов
        /// </summary>
        /// <param name="first">первый массив</param>
        /// <param name="second">второй массив</param>
        /// <returns>1, 0, -1</returns>
        public static int CompareByteArrays(byte[] first, byte[] second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("входные параметры не должны быть нулами");
            if (first.Length != second.Length)
                throw new ArgumentException("аргументы неравной длины");
            for (int i = 0; i < first.Length; ++i)
            {
                if (first[i] > second[i]) return +1;
                if (first[i] < second[i]) return -1;
            }
            return 0;
        }

        /// <summary>
        /// копирование байт
        /// </summary>
        /// <param name="Dest">куда</param>
        /// <param name="Source">откуда</param>
        /// <param name="DestStartIndex">стартовый индекс куда копировать</param>
        /// <param name="Length">длина блока</param>
        /// <returns>массив, куда скопировали</returns>
        public static byte[] CopyBytes(byte[] Dest, byte[] Source, int DestStartIndex, int Length)
        {
            for (int i = DestStartIndex; i < DestStartIndex + Length; ++i) Dest[i] = Source[i - DestStartIndex];
            return Dest;
        }

        public static string GetQuarterString(int Month)
        {
            if (1 <= Month && Month <= 3) return "I";
            else if (4 <= Month && Month <= 6) return "II";
            else if (7 <= Month && Month <= 9) return "III";
            else if (10 <= Month && Month <= 12) return "IV";
            else return "---";
        }

        public static string GetDateString(GroupDateEnum Enum, int DateValue)
        {
            switch (Enum)
            {
                case GroupDateEnum.Year:
                    return DateValue.ToString();
                case GroupDateEnum.Month:
                    switch (DateValue)
                    {
                        case 1: return "Январь";
                        case 2: return "Февраль";
                        case 3: return "Март";
                        case 4: return "Апрель";
                        case 5: return "Май";
                        case 6: return "Июнь";
                        case 7: return "Июль";
                        case 8: return "Август";
                        case 9: return "Сентябрь";
                        case 10: return "Октябрь";
                        case 11: return "Ноябрь";
                        case 12: return "Декабрь";
                        default: throw new Exception("Неверное значение для месяца (1-12): " + DateValue.ToString());
                    }
                case GroupDateEnum.Day:
                    if (DateValue < 1 || DateValue > 31) throw new Exception("Неверное значение для дня: " + DateValue.ToString());
                    return DateValue.ToString();
                case GroupDateEnum.No:
                    return DateValue.ToString();
                default: throw new Exception("В перечислении появилось новое значение.");
            }
        }

        public static string GetGroupDateString(GroupDateEnum Enum)
        {
            switch (Enum)
            {
                case GroupDateEnum.Year: return "Годовая";
                case GroupDateEnum.Month: return "Месячная";
                default: throw new Exception("В перечислении появилось новое значение.");
            }
        }

        /// <summary>
        /// поддерживает ли данный тип данный интерфейс
        /// </summary>
        /// <param name="CheckedType">тип</param>
        /// <param name="InterfaceType">интерфейс</param>
        /// <returns>true, если да</returns>
        public static bool InterfaceSupported(Type CheckedType, Type InterfaceType)
        {
            List<Type> interfaces = new List<Type>(CheckedType.GetInterfaces());
            foreach (Type InType in interfaces) if (InType.Equals(InterfaceType)) return true;
            return false;
        }

        /// <summary>
        /// перечисляет гуиды, применяя к каждому операцию CAST
        /// </summary>
        /// <param name="Collection">коллекция гуидов</param>
        /// <returns>строка в формате CAST('guid1'), CAST('guid2'), CAST('guid3')</returns>
        public static string EnumerateGuidsInDbForm(IEnumerable<Guid> Collection)
        {
            if (Collection.Count() == 0) return null;
            StringBuilder sb = new StringBuilder((Collection.Count() + 5) * 32);
            foreach (Guid g in Collection) sb.Append("CAST('").Append(g.ToString("B")).Append("' as System.Guid), ");
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        #endregion

    }

    /// <summary>
    /// Компаратор для массива гуидов.
    /// </summary>
    public class GuidArrayComparer : IEqualityComparer<Guid[]>
    {

        /// <summary>
        /// сравнить два массива, порядок расположения идентификаторов важен
        /// </summary>
        /// <param name="x">первый массив</param>
        /// <param name="y">второй массив</param>
        /// <returns>true, если массивы равны</returns>
        public bool Equals(Guid[] x, Guid[] y)
        {
            if (x == null && y == null) return true;
            if ((x != null && y == null) || (x == null && y != null)) return false;
            if (object.ReferenceEquals(x, y)) return true;
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; ++i) if (x[i] != y[i]) return false;
            return true;
        }

        /// <summary>
        /// хеш-код
        /// </summary>
        /// <param name="obj">массив гуидов</param>
        /// <returns>значение хеш-кода</returns>
        public int GetHashCode(Guid[] obj)
        {
            int y = 0;
            foreach (Guid g in obj) y = y ^ g.GetHashCode();
            return y;
        }

    }

    /// <summary>
    /// Класс расширенного гуида.
    /// </summary>
    public class ExtendedGuid : IComparable, IComparable<ExtendedGuid>, IEquatable<ExtendedGuid>
    {

        #region Поля

        private static int COUNT = 28;
        private static byte[] empty = new byte[COUNT];
        private byte[] storage;

        /// <summary>
        /// пустой гуид
        /// </summary>
        public static readonly ExtendedGuid Empty = new ExtendedGuid(empty);

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public ExtendedGuid() { storage = STD.CopyBytes(new byte[COUNT], Guid.NewGuid().ToByteArray(), 0, 16); }

        /// <summary>
        /// конструктор с массивом
        /// </summary>
        /// <param name="b">байтовый массив - основа гуида</param>
        public ExtendedGuid(byte[] b)
        {
            if (b == null) throw new ArgumentNullException("Входной массив не задан");
            if (b.Length > COUNT) throw new ArgumentException("Длина входного массива не должна превышать " + COUNT.ToString() + " байт");
            storage = new byte[COUNT];
            STD.CopyBytes(storage, b, 0, b.Length);
        }

        /// <summary>
        /// конструктор с гуидом
        /// </summary>
        /// <param name="g">гуид (16 байт)</param>
        public ExtendedGuid(Guid g) : this(g.ToByteArray()) { }

        #endregion

        #region Методы

        /// <summary>
        /// сгенерировать уникальный идентификатор
        /// </summary>
        /// <returns>новый идентификатор</returns>
        public static ExtendedGuid NewGuid()
        {
            int guid_count = COUNT / 16; // целое число гуидов, содержащихся в AdminGuid
            byte[] bytes = new byte[COUNT];
            for (int i = 0; i < COUNT / 16; ++i)
                STD.CopyBytes(bytes, Guid.NewGuid().ToByteArray(), i * 16, 16);
            if (guid_count * 16 == COUNT) return new ExtendedGuid(bytes);
            return new ExtendedGuid(STD.CopyBytes(bytes, Guid.NewGuid().ToByteArray(), guid_count * 16, COUNT - (guid_count * 16)));
        }

        /// <summary>
        /// возвращает копию внутреннего представления
        /// </summary>
        /// <returns></returns>
        public byte[] GetBinaryValue() { return GetBinaryValue(COUNT); }

        /// <summary>
        /// возвращает копию части внутреннего представления
        /// </summary>
        /// <param name="Count">число байт, которое будет возвращено</param>
        /// <returns>массив из Count байт</returns>
        public byte[] GetBinaryValue(int Count)
        {
            if (Count > COUNT || Count < 0) throw new ArgumentException("Параметр Count должен быть неотрицательным и меньшим " + COUNT.ToString());
            byte[] result = new byte[Count];
            for (int i = 0; i < Count; ++i) result[i] = storage[i];
            return result;
        }

        /// <summary>
        /// получить хеш-значение
        /// </summary>
        /// <returns>int, сгенерированный с помощью CRC</returns>
        public override int GetHashCode() { return (int)CRC32.Crc32(storage); }

        /// <summary>
        /// получить текстовое представление идентификатора
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return ByteConverterAdv.Convert(storage, Digits.Dec); }

        /// <summary>
        /// сравнить 2 гуида
        /// </summary>
        /// <param name="obj">чего сравнивать</param>
        /// <returns>1, 0, -1</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is ExtendedGuid)) throw new ArgumentException("obj должен иметь тип ExtendedGuid или производный от него");
            return STD.CompareByteArrays(storage, (obj as ExtendedGuid).storage);
        }

        /// <summary>
        /// сравнить 2 гуида
        /// </summary>
        /// <param name="other">чего сравнивать</param>
        /// <returns>1, 0, -1</returns>
        public int CompareTo(ExtendedGuid other) { return STD.CompareByteArrays(storage, other.storage); }

        /// <summary>
        /// эквивалентность двух гуидов
        /// </summary>
        /// <param name="other">чего сравнивать</param>
        /// <returns>true, если равны</returns>
        public bool Equals(ExtendedGuid other) { return STD.IsByteArraysEqual(storage, other.storage); }

        #endregion

    }

    /// <summary>
    /// Класс получения 32-битной хеш-функции по массиву байт.
    /// </summary>
    public static class CRC32
    {

        #region Поля

        private static readonly uint[] Crc32_PolyVals = 
		{
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
            0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
            0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
            0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
            0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
            0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
            0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
            0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
            0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
            0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
            0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
            0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
            0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
            0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
            0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D,
		};

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш (начальное значение = 0)
        /// </summary>
        /// <param name="StartingCRC">начальное значение</param>
        /// <param name="Bytes">массив байт, по которому надо получить хеш</param>
        /// <param name="StartingIndex">начальный индекс в массиве для хеша</param>
        /// <param name="Count">число байт для хеша</param>
        /// <returns>интовый результат</returns>
        public static uint Crc32(uint StartingCRC, byte[] Bytes, int StartingIndex, int Count)
        {
            uint iCursor = ~StartingCRC;
            int iTop = StartingIndex + Count;
            for (int i = StartingIndex; i < iTop; ++i)
                iCursor = (((uint)(iCursor / 256)) & 0xFFFFFF) ^ Crc32_PolyVals[(int)((iCursor ^ Bytes[i]) & 0xFF)];
            return ~iCursor;
        }

        /// <summary>
        /// получить хеш (начальное значение = 0)
        /// </summary>
        /// <param name="Bytes">массив байт, по которому надо получить хеш</param>
        /// <returns>интовый результат</returns>
        public static uint Crc32(byte[] Bytes)
        {
            return Crc32(0, Bytes, 0, Bytes.Length);
        }

        #endregion

    }

    /// <summary>
    /// Класс пары значений.
    /// </summary>
    /// <typeparam name="T">тип первого значения</typeparam>
    /// <typeparam name="U">тип второго значения</typeparam>
    public class Pair<T, U>
    {

        #region Поля

        private T _key = default(T);
        private U _value = default(U);

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public Pair() { }

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="Key">первое значение пары</param>
        /// <param name="Value">второе значение пары</param>
        public Pair(T Key, U Value) : this() { _key = Key; _value = Value; }

        #endregion

        #region Свойства

        /// <summary>
        /// получить/установить первое значение пары
        /// </summary>
        public T Key { get { return _key; } set { _key = value; } }

        /// <summary>
        /// получить/установить второе значение пары
        /// </summary>
        public U Value { get { return _value; } set { _value = value; } }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Pair<T, U>)
            {
                var otherPair = obj as Pair<T, U>;
                return Key.Equals(otherPair.Key) && Value.Equals(otherPair.Value);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Value.GetHashCode();
        }
    }

    /// <summary>
    /// Класс для ассоциатора-генератора гуидов.
    /// </summary>
    public class GuidIntGen : BidirectionalDictionary<int, Guid>
    {

        /// <summary>
        /// конструктор генератора
        /// </summary>
        public GuidIntGen() : base() { }

        /// <summary>
        /// получить гуид
        /// </summary>
        /// <param name="Value">целочисленное значение, для которого нужно получить гуид</param>
        /// <returns>уже существующий гуид из словаря, если ассоциация уже есть, или новое сгенерированное значение</returns>
        public Guid GetGuid(int Value)
        {
            if (Exists(Value)) return this[Value];
            else
            {
                Guid res = Guid.NewGuid();
                AddAssociation(Value, res);
                return res;
            }
        }
    }

    /// <summary>
    /// Словарь guid`ов с двумя ключами.
    /// </summary>
    public class TwiceGuidAssociator : BidirectionalDictionary<Pair<Guid, Guid>, Guid>
    {

        /// <summary>
        /// Добавить запись
        /// </summary>
        /// <param name="First">Первый ключ</param>
        /// <param name="Second">Первый ключ</param>
        /// <param name="Value">Значение</param>
        public void Add(Guid First, Guid Second, Guid Value)
        {
            AddAssociation(new Pair<Guid, Guid>(First, Second), Value);
        }

        /// <summary>
        /// Вернуть значение. Если записи нет, то она будет сначала создана со значением Guid.NewGuid()
        /// </summary>
        /// <param name="FirstId"></param>
        /// <param name="SecondId"></param>
        /// <returns></returns>
        public Guid ProcessGuid(Guid FirstId, Guid SecondId)
        {
            var pair = new Pair<Guid, Guid>(FirstId, SecondId);
            if (Exists(pair)) return this[pair];
            else
            {
                var id = Guid.NewGuid();
                AddAssociation(pair, id);
                return id;
            }
        }

    }

}