using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace LuckyProject.Lib.Basics.Services
{
    public class ConsoleService : IConsoleService
    {
        #region Console
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public TextReader In => Console.In;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public Encoding InputEncoding
        {
            get => Console.InputEncoding;
            set => Console.InputEncoding = value;
        }

        public Encoding OutputEncoding
        {
            get => Console.OutputEncoding;
            [UnsupportedOSPlatform("android")]
            [UnsupportedOSPlatform("ios")]
            [UnsupportedOSPlatform("tvos")]
            set => Console.OutputEncoding = value;
        }

        public bool KeyAvailable => Console.KeyAvailable;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public ConsoleKeyInfo ReadKey() => Console.ReadKey();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

        public TextWriter Out => Console.Out;
        public TextWriter Error => Console.Error;

        public bool IsInputRedirected => Console.IsInputRedirected;
        public bool IsOutputRedirected => Console.IsOutputRedirected;
        public bool IsErrorRedirected => Console.IsErrorRedirected;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int CursorSize => Console.CursorSize;

        [SupportedOSPlatform("windows")]
        public bool NumberLock => Console.NumberLock;

        [SupportedOSPlatform("windows")]
        public bool CapsLock => Console.CapsLock;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public void ResetColor() => Console.ResetColor();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int BufferWidth => Console.BufferWidth;
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int BufferHeight => Console.BufferWidth;

        [SupportedOSPlatform("windows")]
        public void SetBufferSize(int width, int height) => Console.SetBufferSize(width, height);

        public int WindowLeft => Console.WindowLeft;
        public int WindowTop => Console.WindowTop;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int WindowWidth
        { 
            get => Console.WindowWidth;
            set => Console.WindowWidth = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int WindowHeight
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }

        [SupportedOSPlatform("windows")]
        public void SetWindowPosition(int left, int top) => Console.SetWindowPosition(left, top);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int LargestWindowWidth => Console.LargestWindowWidth;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int LargestWindowHeight => Console.LargestWindowHeight;

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public bool CursorVisible
        {
            set => Console.CursorVisible = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public (int Left, int Top) GetCursorPosition() => Console.GetCursorPosition();
        public string Title
        {
            [UnsupportedOSPlatform("android")]
            [UnsupportedOSPlatform("browser")]
            [UnsupportedOSPlatform("ios")]
            [UnsupportedOSPlatform("tvos")]
            set => Console.Title = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public void Beep() => Console.Beep();

        [SupportedOSPlatform("windows")]
        public void Beep(int frequency, int duration) => Console.Beep(frequency, duration);

        [SupportedOSPlatform("windows")]
        public void MoveBufferArea(
            int sourceLeft,
            int sourceTop,
            int sourceWidth,
            int sourceHeight,
            int targetLeft,
            int targetTop) =>
            Console.MoveBufferArea(
                sourceLeft,
                sourceTop,
                sourceWidth,
                sourceHeight,
                targetLeft,
                targetTop);

        [SupportedOSPlatform("windows")]
        public void MoveBufferArea(
            int sourceLeft,
            int sourceTop,
            int sourceWidth,
            int sourceHeight,
            int targetLeft,
            int targetTop,
            char sourceChar,
            ConsoleColor sourceForeColor,
            ConsoleColor sourceBackColor) =>
            Console.MoveBufferArea(
                sourceLeft,
                sourceTop,
                sourceWidth,
                sourceHeight,
                targetLeft,
                targetTop,
                sourceChar,
                sourceForeColor,
                sourceBackColor);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public void Clear() => Console.Clear();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add => Console.CancelKeyPress += value;
            remove => Console.CancelKeyPress -= value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public bool TreatControlCAsInput
        {
            get => Console.TreatControlCAsInput;
            set => Console.TreatControlCAsInput = value;
        }

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public Stream OpenStandardInput() => Console.OpenStandardInput();

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        public Stream OpenStandardInput(int bufferSize) => Console.OpenStandardInput(bufferSize);

        public Stream OpenStandardOutput() => Console.OpenStandardOutput();
        public Stream OpenStandardOutput(int bufferSize) => Console.OpenStandardOutput(bufferSize);

        public Stream OpenStandardError() => Console.OpenStandardError();
        public Stream OpenStandardError(int bufferSize) => Console.OpenStandardError(bufferSize);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        [UnsupportedOSPlatform("ios")]
        [UnsupportedOSPlatform("tvos")]
        public void SetIn(TextReader newIn) => Console.SetIn(newIn);
        public void SetOut(TextWriter newOut) => Console.SetOut(newOut);
        public void SetError(TextWriter newError) => Console.SetError(newError);

        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        public int Read() => Console.Read();
        [UnsupportedOSPlatform("android")]
        [UnsupportedOSPlatform("browser")]
        public string ReadLine() => Console.ReadLine();

        public void WriteLine() => Console.WriteLine();
        public void WriteLine(bool value) => Console.WriteLine(value);
        public void WriteLine(char value) => Console.WriteLine(value);
        public void WriteLine(char[] buffer) => Console.WriteLine(buffer);
        public void WriteLine(char[] buffer, int index, int count) =>
            Console.WriteLine(buffer, index, count);
        public void WriteLine(decimal value) => Console.WriteLine(value);
        public void WriteLine(double value) => Console.WriteLine(value);
        public void WriteLine(float value) => Console.WriteLine(value);
        public void WriteLine(int value) => Console.WriteLine(value);
        public void WriteLine(uint value) => Console.WriteLine(value);
        public void WriteLine(long value) => Console.WriteLine(value);
        public void WriteLine(ulong value) => Console.WriteLine(value);
        public void WriteLine(object value) => Console.WriteLine(value);
        public void WriteLine(string value) => Console.WriteLine(value);
        public void WriteLine(string format, object arg0) => Console.WriteLine(format, arg0);
        public void WriteLine(string format, object arg0, object arg1) =>
             Console.WriteLine(format, arg0, arg1);
        public void WriteLine(string format, object arg0, object arg1, object arg2) =>
            Console.WriteLine(format, arg0, arg1, arg2);
        public void WriteLine(string format, params object[] arg) => Console.WriteLine(format, arg);

        public void Write(string format, object arg0) => Console.Write(format, arg0);
        public void Write(string format, object arg0, object arg1) =>
            Console.WriteLine(format, arg0, arg1);
        public void Write(string format, object arg0, object arg1, object arg2) =>
            Console.WriteLine(format, arg0, arg1, arg2);
        public void Write(string format, params object[] arg) => Console.Write(format, arg);
        public void Write(bool value) => Console.Write(value);
        public void Write(char value) => Console.WriteLine(value);
        public void Write(char[] buffer) => Console.Write(buffer);
        public void Write(char[] buffer, int index, int count) =>
            Console.Write(buffer, index, count);
        public void Write(double value) => Console.Write(value);
        public void Write(decimal value) => Console.Write(value);
        public void Write(float value) => Console.Write(value);
        public void Write(int value) => Console.Write(value);
        public void Write(uint value) => Console.Write(value);
        public void Write(long value) => Console.Write(value);
        public void Write(ulong value) => Console.Write(value);
        public void Write(object value) => Console.Write(value);
        public void Write(string value) => Console.Write(value);
        #endregion

        #region Custom helpers
        public void ClearConsoleLine(int top = -1)
        {
            var prevLeft = CursorLeft;
            var prevTop = CursorTop;
            SetCursorPosition(0, top != -1 ? top : prevTop);
            Write(new string(' ', WindowWidth));
            SetCursorPosition(prevLeft, prevTop);
        }
        #endregion
    }
}
