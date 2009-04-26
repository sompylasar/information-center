using System;
using System.Runtime.InteropServices;

namespace LogicUtils
{

    public static class DllExport
    {

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceCounter", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(ref long lPerformanceCounter);

        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceFrequency", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(out long frequency);

        public static long Frequency
        {
            get
            {
                long result = 0;
                if (QueryPerformanceFrequency(out result)) return result;
                else return -1;
            }
        }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX() { dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)); }
    }

    //[CLSCompliant(false)]
    public class MemoryInformation
    {

        #region Поля

        private MEMORYSTATUSEX mem_stat_ex = new MEMORYSTATUSEX();

        #endregion

        #region Конструкторы

        public MemoryInformation() { }

        #endregion

        #region Свойства

        public uint MemoryLoad { get { return mem_stat_ex.dwMemoryLoad; } }

        public ulong AvailPhys { get { return mem_stat_ex.ullAvailPhys; } }

        public ulong AvailVirtual { get { return mem_stat_ex.ullAvailVirtual; } }

        #endregion

        #region Методы

        public MemoryInformation Request()
        {
            if (!DllExport.GlobalMemoryStatusEx(mem_stat_ex)) throw new Exception("Инициализация GlobalMemoryStatusEx API не удалась.");
            return this;
        }

        #endregion

    }    

}