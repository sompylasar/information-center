using System;
using System.Collections.Generic;

namespace LogicUtils
{

    /// <summary>
    /// Словарь с двояким доступом
    /// </summary>
    /// <typeparam name="TFirstKey">Тип первого ключа</typeparam>
    /// <typeparam name="TSecondKey">Тип второго ключа</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <remarks>При совпадении типов TFirstKey и TSecondKey работать не будет</remarks>
    public class DualAccessKeyDictionary<TFirstKey, TSecondKey, TValue>
    {
        /// <summary>
        /// Добавить запись
        /// </summary>
        /// <param name="firstKey">Первый ключ</param>
        /// <param name="secondKey">Второй ключ</param>
        /// <param name="value">Значение</param>
        public void Add(TFirstKey firstKey, TSecondKey secondKey, TValue value)
        {
            if (!FirstDict.ContainsKey(firstKey) && !SecondDict.ContainsKey(secondKey))
            {
                FirstDict.Add(firstKey, value);
                SecondDict.Add(secondKey, value);
            }
        }
        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="firstKey">Первый ключ</param>
        /// <param name="secondKey">Второй ключ</param>
        public void Remove(TFirstKey firstKey, TSecondKey secondKey)
        {
            if (FirstDict.ContainsKey(firstKey) && SecondDict.ContainsKey(secondKey))
            {
                FirstDict.Remove(firstKey);
                SecondDict.Remove(secondKey);
            }
        }
        /// <summary>
        /// Очистить словарь
        /// </summary>
        public void Clear()
        {
            FirstDict.Clear();
            SecondDict.Clear();
        }
        /// <summary>
        /// Проверить существование ключа (записи)
        /// </summary>
        /// <param name="key">Первый ключ</param>
        /// <returns>   true    - ключ найден
        ///             false   - ключ не найден</returns>
        public bool ContainsKey(TFirstKey key)
        {
            return FirstDict.ContainsKey(key);
        }
        /// <summary>
        /// Проверить существование ключа (записи)
        /// </summary>
        /// <param name="key">Второй ключ</param>
        /// <returns>   true    - ключ найден
        ///             false   - ключ не найден</returns>
        public bool ContainsKey(TSecondKey key)
        {
            return SecondDict.ContainsKey(key);
        }
        /// <summary>
        /// Индексатор доступа (только чтение)
        /// </summary>
        /// <param name="index">Первый ключ</param>
        /// <returns>Значение</returns>
        public TValue this[TFirstKey index]
        {
            get { return FirstDict[index]; }
        }
        /// <summary>
        /// Индексатор доступа (только чтение)
        /// </summary>
        /// <param name="index">Второй ключ</param>
        /// <returns>Значение</returns>
        public TValue this[TSecondKey index]
        {
            get { return SecondDict[index]; }
        }
        /// <summary>
        /// Индексатор доступа
        /// </summary>
        /// <param name="firstIndex">Первый ключ</param>
        /// <param name="secondIndex">Второй ключ</param>
        /// <returns>Значение</returns>
        public TValue this[TFirstKey firstIndex, TSecondKey secondIndex]
        {
            get { return FirstDict[firstIndex]; }
            set
            {
                if (FirstDict.ContainsKey(firstIndex) && SecondDict.ContainsKey(secondIndex))
                {
                    FirstDict[firstIndex] = value;
                    SecondDict[secondIndex] = value;
                }
                else
                    throw new IndexOutOfRangeException("First and second indexies must contains in the dictionary.");
            }
        }
        /// <summary>
        /// Коллекция значений
        /// </summary>
        public Dictionary<TFirstKey, TValue>.ValueCollection Values
        {
            get { return FirstDict.Values; }
        }
        /// <summary>
        /// Коллекция первых ключей
        /// </summary>
        public Dictionary<TFirstKey, TValue>.KeyCollection FirstKeys
        {
            get { return FirstDict.Keys; }
        }
        /// <summary>
        /// Коллекция вторых ключей
        /// </summary>
        public Dictionary<TSecondKey, TValue>.KeyCollection SecondKeys
        {
            get { return SecondDict.Keys; }
        }

        private Dictionary<TFirstKey, TValue> FirstDict = new Dictionary<TFirstKey, TValue>();
        private Dictionary<TSecondKey, TValue> SecondDict = new Dictionary<TSecondKey, TValue>();
    }

}