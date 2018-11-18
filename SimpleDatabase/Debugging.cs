using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Database.Internal
{
    internal class Debugging
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        public static void Print(string s)
        {
#if DEBUG
            Debug.Print(s);
#else
#if TRACE
            Debug.Print(s);
#else
            OutputDebugString(s);
#endif
#endif
        }
    }
}
