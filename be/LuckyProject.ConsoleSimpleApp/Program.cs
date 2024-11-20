using LuckyProject.Lib.Basics.LiveObjects.Processes;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Init;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleSimpleApp
{
    internal class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Interoperability",
            "CA1416:Validate platform compatibility",
            Justification = "<Pending>")]
        static void Main(string[] args)
        {
            InitAssembly.Init();
            var bytes = new byte[64];
            var rnd = new Random();
            rnd.NextBytes(bytes);
            var sb = new StringBuilder("var bbb = new byte[] { ");
            for (int i = 0; i < 64; ++i)
            {
                sb.Append($"0x{bytes[i]:X2}, ");
            }
            sb.Append("};");
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
        }
    }
}
