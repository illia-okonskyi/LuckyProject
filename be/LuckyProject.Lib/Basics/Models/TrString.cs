using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Models
{
    public class TrString
    {
        public string Format { get; init; } = string.Empty;
        public List<string> Args { get; init; } = new();

        public TrString()
        { }

        public TrString(string format, List<string> args = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format));
            }

            Format = format;
            Args.AddRange(args.EmptyIfNull());
        }

        public override string ToString()
        {
            return string.Format(Format, Args);
        }
    }
}
