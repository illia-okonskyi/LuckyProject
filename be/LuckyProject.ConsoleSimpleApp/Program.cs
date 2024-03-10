using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using Newtonsoft.Json;
using System;

namespace LuckyProject.ConsoleSimpleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s = "1\r\n2\n\n4.txt\n444";
            var l = s.SplitByLines();
            var pl = l.ToPaginatedList(2, 2);
            var s2 = JsonConvert.SerializeObject(pl, Formatting.Indented);
            var pl2 = JsonConvert.DeserializeObject<PaginatedList<string>>(s2);
            Console.WriteLine(s.ToSurrounded(CommonStringSurround.SingleQuotes));
            Console.ReadKey();
        }
    }
}
