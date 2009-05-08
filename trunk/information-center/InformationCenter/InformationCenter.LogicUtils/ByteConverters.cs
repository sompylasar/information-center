using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

namespace LogicUtils
{

    /// <summary>
    /// Тип основания для систем счисления.
    /// </summary>
    public enum Digits
    {

        /// <summary>
        /// основание 2
        /// </summary>
        [Description("Двоичное значение")]
        Bin,

        /// <summary>
        /// основание 8
        /// </summary>
        [Description("Восьмеричное значение")]
        Oct,

        /// <summary>
        /// основание 10
        /// </summary>
        [Description("Десятичное значение")]
        Dec,

        /// <summary>
        /// основание 16
        /// </summary>
        [Description("Шестнадцатеричное значение")]
        Hex

    }

    /// <summary>
    /// Класс - конвертер для одного байта. Преобразует число, представленное байтом в строку символов
    /// заданной системы счисления.
    /// </summary>
    public class SingleByteConverter
    {

        #region Поля

        private byte r;
        private byte num;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор конвертера с заданным основанием системы счисления
        /// </summary>
        /// <param name="Radix">основание системы счисления</param>
        public SingleByteConverter(byte Radix) { RefreshRadix(Radix); }

        #endregion

        #region Свойства

        /// <summary>
        /// получить число позиций, необходимых для отображения байтовых значений с заданным в 
        /// конструкторе основанием системы счисления
        /// </summary>
        public int DigitsCount { get { return num; } }

        /// <summary>
        /// получить/установить основание системы счисления
        /// </summary>
        public byte Radix { get { return r; } set { RefreshRadix(value); } }

        #endregion

        #region Методы

        #region PRIVATE

        private bool Valid(char c)
        {
            if (r > 9) return (c >= '0' && c <= '9') || (c >= 'A' && c <= (char)(48 + 7 + r));
            else return c >= '0' && c <= '9';
        }

        private void RefreshRadix(byte NewRadix)
        {
            if (NewRadix == 0 || NewRadix == 1) throw new ArgumentException("2 <= основание <= 255");
            r = NewRadix;
            switch (NewRadix)
            {
                case 2: num = 8; break;
                case 3: num = 6; break;
                case 4:
                case 5:
                case 6: num = 4; break;
                default: num = 3; break;
            }
            if (NewRadix >= 16) num = 2;
        }

        #endregion

        #region PUBLIC

        /// <summary>
        /// сконвертировать значение
        /// </summary>
        /// <param name="Value">байтовое значение для конвертирования</param>
        /// <returns>символьное представление нового значения</returns>
        public string Convert(byte Value)
        {
            StringBuilder b = new StringBuilder(num);
            int mod, counter = 0;
            do
            {
                mod = Value % r;
                b.Insert(0, (char)(48 + (mod > 9 ? mod + 7 : mod))); // +7 надо для перехода к ABCDEF...
                Value /= r;
                ++counter;
            } while (Value > 0);
            for (int i = counter; i < num; ++i) b.Insert(0, '0');
            return b.ToString();
        }

        /// <summary>
        /// сконвертировать строку в байтовое значение
        /// </summary>
        /// <param name="Value">строка символов</param>
        /// <returns>байтовое значение</returns>
        public byte Convert(string Value)
        {
            Value = Value.ToUpper();
            if (Value.Length != num) throw new ArgumentException(Value.Length.ToString() + " != " + num.ToString());
            char[] chars = Value.ToCharArray();
            for (int i = 0; i < chars.Length; ++i)
                if (!Valid(chars[i])) throw new ArgumentException("Неверный символ: " + chars[i]);
            byte result = 0;
            int denom = 1;
            for (int i = 0; i < chars.Length; ++i)
            {
                char c = chars[chars.Length - i - 1];
                result += (byte)(((c >= 'A' ? (int)c - 7 : (int)c) - (int)'0') * denom);
                denom *= r;
            }
            return result;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// Конвертер для байтовых массивов.
    /// </summary>
    public static class ByteConverterAdv
    {

        /// <summary>
        /// конвертирование к заданному основанию системы счисления
        /// </summary>
        /// <param name="Source">исходный байтовый массив значений</param>
        /// <param name="Kind">основание системы счисления</param>
        /// <returns>символьное выражение заданного массива байт в этом основании</returns>
        public static string Convert(byte[] Source, Digits Kind)
        {
            byte radix;
            switch (Kind)
            {
                case Digits.Bin: radix = 2; break;
                case Digits.Oct: radix = 8; break;
                case Digits.Dec: radix = 10; break;
                case Digits.Hex: radix = 16; break;
                default: throw new Exception("Не поддерживаемое основание.");
            }
            SingleByteConverter converter = new SingleByteConverter(radix);
            StringBuilder result = new StringBuilder(Source.Length);
            foreach (byte b in Source) result.Append(converter.Convert(b));
            return result.ToString();
        }

        /// <summary>
        /// сконвертировать строку к байтовому массиву
        /// </summary>
        /// <param name="Source">строка чисел в заданной системе счисления</param>
        /// <param name="Kind">основание системы счисления</param>
        /// <returns>массив байт в заданной системе</returns>
        public static byte[] Convert(string Source, Digits Kind)
        {
            byte radix;
            switch (Kind)
            {
                case Digits.Bin: radix = 2; break;
                case Digits.Oct: radix = 8; break;
                case Digits.Dec: radix = 10; break;
                case Digits.Hex: radix = 16; break;
                default: throw new Exception("Не поддерживаемое основание.");
            }
            SingleByteConverter converter = new SingleByteConverter(radix);
            // не раскрывать скобки!!! тут вычисляется число символов, которыми нужно дополнить строку... способ жжот :)
            int L = Source.Length, N = converter.DigitsCount, dummies = N - L + (L / N) * N - ((N - L + (L / N) * N) / N) * N;
            for (int i = 0; i < dummies; ++i) Source = Source.Insert(0, "0");
            L = Source.Length;
            List<byte> result = new List<byte>();
            for (int i = 0; i < L / N; ++i) result.Add(converter.Convert(Source.Substring(i * N, N)));
            return result.ToArray();
        }

    }

}