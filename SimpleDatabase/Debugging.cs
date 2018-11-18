using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Database.Internal
{
    internal class Debugging
    {
#if DEBUG
        public static void Print(string s)
        {
            Debug.Print(s);
        }

        public static void PrintExceptionMessage(string s)
        {
            Debug.Print(s);
        }
#else
#if TRACE
        public static void Print(string s)
        {
            if (AccumulatedTraces == null)
                AccumulatedTraces = s;
            else
                AccumulatedTraces += "\n" + AccumulatedTraces;

            Debug.Print(s);
        }

        public static void PrintExceptionMessage(string s)
        {
            s = AccumulatedTraces + "\n" + s;
            AccumulatedTraces = null;

            throw new ApplicationException(s);
        }

        private static string AccumulatedTraces;
#else
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        public static void Print(string s)
        {
            OutputDebugString(s);
        }

        public static void PrintExceptionMessage(string s)
        {
            OutputDebugString(s);
        }
#endif
#endif
    }
}
