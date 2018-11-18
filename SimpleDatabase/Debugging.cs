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
            Debug.Print(s);
        }

        public static void PrintExceptionMessage(string s)
        {
            throw new ApplicationException(s);
        }
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
