using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.ConsoleTool.Helpers
{
    public static class CommandLineArgsHelper
    {
        public static bool IsHelpRequested(
            IEnumerable<string> args,
            bool firstArgIsExecutable = true)
        {
            var noArgsPassed = args.SafeCount() < (firstArgIsExecutable ? 2 : 1);
            return noArgsPassed || args.Contains("-h") || args.Contains("--help");
        }
    }
}
