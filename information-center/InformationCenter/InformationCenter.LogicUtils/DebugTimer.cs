using System;
using System.Diagnostics;

namespace LogicUtils
{

    /// <summary>
    /// Таймер.
    /// </summary>
    public class DebugTimer : IDisposable
    {

        #region Поля

        private long start = 0, current = 0;
        private bool init_freq = false;
        private double freq = 0.0;
        private string stored_format = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public DebugTimer() : this(true) { }

        /// <summary>
        /// конструктор с указанием инициализации частоты
        /// </summary>
        /// <param name="InitFrequency">инициализировать ли счётчик частоты работы один раз</param>
        public DebugTimer(bool InitFrequency)
        {
            init_freq = InitFrequency;
            if (init_freq) freq = (double)DllExport.Frequency;
        }

        /// <summary>
        /// конструктор со строкой формата (для вызова в Dispose)
        /// </summary>
        /// <param name="Format">строка формата</param>
        public DebugTimer(string Format) : this()
        {
            stored_format = Format;
            Start();
        }

        #endregion

        #region Методы

        /// <summary>
        /// запустить таймер
        /// </summary>
        public void Start() { DllExport.QueryPerformanceCounter(ref start); }

        /// <summary>
        /// получить текущее число секунд, прошедших с последнего вызова старта таймера
        /// </summary>
        /// <returns>значение double</returns>
        public double GetSeconds()
        {
            DllExport.QueryPerformanceCounter(ref current);
            return (current - start) / (init_freq ? freq : (double)DllExport.Frequency);
        }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>время в секундах</returns>
        public override string ToString() { return GetSeconds().ToString("F4"); }

        /// <summary>
        /// преобразовать в отладочную строку
        /// </summary>
        /// <param name="Format">форматирующая строка</param>
        [Conditional("DEBUG")]
        public void ToTraceString(string Format)
        {
            Debug.Indent();
            Debug.WriteLine(string.Format(Format, ToString()));
            Debug.Unindent();
        }

        /// <summary>
        /// преобразовать в отладочную строку, используя формат при создании
        /// </summary>
        [Conditional("DEBUG")]
        protected void ToTraceString() { ToTraceString(stored_format);  }

        /// <summary>
        /// освободить ресурсы
        /// </summary>
        public void Dispose() { ToTraceString(); }

        #endregion

    }

}