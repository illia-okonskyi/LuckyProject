using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuckyProject.Lib.Basics.LiveObjects.Processes
{
    public class LpProcessStartParams
    {
        public string FileName { get; init; }
        public List<string> Arguments { get; init; } = new();
        public string WorkingDir { get; init; } = string.Empty;
        public Dictionary<string, string> Environment { get; init; } = new();
        public Encoding Encoding { get; init; }

        public override string ToString()
        {
            var env = Environment.Select(kvp => $"  [{kvp.Key}] = {kvp.Value}").ToList();

            var sb = new StringBuilder("LpConsoleProcessStartParams <");
            sb.Append(FileName);
            sb.AppendLine(">:");
            sb.Append("Working Dir: ");
            sb.AppendLine(WorkingDir);
            sb.Append("Encoding: ");
            sb.AppendLine(this.Encoding?.EncodingName ?? "Default");
            sb.AppendLine("ARGS:");
            for (int i = 0; i < Arguments.Count; ++i)
            {
                sb.Append("  #");
                sb.Append(i);
                sb.Append(": ");
                sb.Append(Arguments[i]);
            }
            sb.AppendLine("ENV:");
            env.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }
    }
}
