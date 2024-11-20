using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Imports
{
    public static class LpImports
    {
        public static class Windows
        {
            public const int CTRL_C_EVENT = 0;
            public delegate bool ConsoleCtrlDelegate(uint CtrlType);

            [DllImport("windows-kernel32.dll", SetLastError = true)]
            public static extern bool SetConsoleCtrlHandler(
                ConsoleCtrlDelegate HandlerRoutine,
                bool Add);

            [DllImport("windows-kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GenerateConsoleCtrlEvent(
                uint dwCtrlEvent,
                uint dwProcessGroupId);

            [DllImport("windows-kernel32.dll", SetLastError = true)]
            public static extern bool AttachConsole(uint dwProcessId);

            [DllImport("windows-kernel32.dll", SetLastError = true)]
            public static extern bool FreeConsole();
        }

        public static class Linux
        {
            [DllImport("linux-libc", SetLastError = true)]
            public static extern int kill(int pid, int sig);
        }
    }
}
