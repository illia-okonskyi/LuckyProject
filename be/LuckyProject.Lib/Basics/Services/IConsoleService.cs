using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IConsoleService
    {
        #region Console
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        TextReader In { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        Encoding InputEncoding { get; set; }

        Encoding OutputEncoding
        {
            get;
            [UnsupportedOSPlatform("android")]
            [UnsupportedOSPlatform("ios")]
            [UnsupportedOSPlatform("tvos")]
            set;
        }

        bool KeyAvailable { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        ConsoleKeyInfo ReadKey();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        ConsoleKeyInfo ReadKey(bool intercept);

        TextWriter Out { get; }

        TextWriter Error { get; }

        bool IsInputRedirected { get; }
        bool IsOutputRedirected { get; }
        bool IsErrorRedirected { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int CursorSize { get; }

        [SupportedOSPlatform("windows")]
        bool NumberLock { get; }

        [SupportedOSPlatform("windows")]
        bool CapsLock { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        ConsoleColor BackgroundColor { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        ConsoleColor ForegroundColor { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        void ResetColor();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int BufferWidth { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int BufferHeight { get; }

        [SupportedOSPlatform("windows")]
        void SetBufferSize(int width, int height);

        int WindowLeft { get; }

        int WindowTop { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int WindowWidth { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int WindowHeight { get; set; }

        [SupportedOSPlatform("windows")]
        void SetWindowPosition(int left, int top);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int LargestWindowWidth { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int LargestWindowHeight { get; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        bool CursorVisible { set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int CursorLeft { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        int CursorTop { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        (int Left, int Top) GetCursorPosition();
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        string Title { set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        void Beep();

        [SupportedOSPlatform("windows")]
        void Beep(int frequency, int duration);

        [SupportedOSPlatform("windows")]
        void MoveBufferArea(
            int sourceLeft,
            int sourceTop,
            int sourceWidth,
            int sourceHeight,
            int targetLeft,
            int targetTop);

        [SupportedOSPlatform("windows")]
        void MoveBufferArea(
            int sourceLeft,
            int sourceTop,
            int sourceWidth,
            int sourceHeight,
            int targetLeft,
            int targetTop,
            char sourceChar,
            ConsoleColor sourceForeColor,
            ConsoleColor sourceBackColor);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        void Clear();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        void SetCursorPosition(int left, int top);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        event ConsoleCancelEventHandler CancelKeyPress;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        bool TreatControlCAsInput { get; set; }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        Stream OpenStandardInput();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        Stream OpenStandardInput(int bufferSize);
        Stream OpenStandardOutput();
        Stream OpenStandardOutput(int bufferSize);
        Stream OpenStandardError();
        Stream OpenStandardError(int bufferSize);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        void SetIn(TextReader newIn);
        void SetOut(TextWriter newOut);
        void SetError(TextWriter newError);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        int Read();
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        string ReadLine();

        void WriteLine();
        void WriteLine(bool value);
        void WriteLine(char value);
        void WriteLine(char[] buffer);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(decimal value);
        void WriteLine(double value);
        void WriteLine(float value);
        void WriteLine(int value);
        void WriteLine(uint value);
        void WriteLine(long value);
        void WriteLine(ulong value);
        void WriteLine(object value);
        void WriteLine(string value);
        void WriteLine(string format, object arg0);
        void WriteLine(string format, object arg0, object arg1);
        void WriteLine(string format, object arg0, object arg1, object arg2);
        void WriteLine(string format, params object[] arg);

        void Write(string format, object arg0);
        void Write(string format, object arg0, object arg1);
        void Write(string format, object arg0, object arg1, object arg2);
        void Write(string format, params object[] arg);
        void Write(bool value);
        void Write(char value);
        void Write(char[] buffer);
        void Write(char[] buffer, int index, int count);
        void Write(double value);
        void Write(decimal value);
        void Write(float value);
        void Write(int value);
        void Write(uint value);
        void Write(long value);
        void Write(ulong value);
        void Write(object value);
        void Write(string value);
        #endregion

        #region Custom Helpers
        /// <summary>
        /// If <paramref name="top"/> equals to -1 current line is cleared
        /// </summary>
        void ClearConsoleLine(int top = -1);
        #endregion
    }
}
