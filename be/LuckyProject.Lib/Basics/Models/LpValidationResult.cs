using System.Collections.Generic;
using System.Text;

namespace LuckyProject.Lib.Basics.Models
{
    public class LpValidationResult
    {
        public bool IsCancelled { get; init; }
        public bool IsValid => !IsCancelled && Errors.Count == 0;
        public List<LpValidationError> Errors { get; init; } = new();

        public override string ToString()
        {
            if (IsCancelled)
            {
                return "LpValidationResult(Cancelled)";
            }
            
            if (IsValid)
            {
                return "LpValidationResult(Valid)";
            }
            
            var sb = new StringBuilder("LpValidationResult(Invalid):");
            sb.AppendLine();
            foreach(var error in Errors)
            {
                sb.AppendLine($"- {error}");
            }
            return sb.ToString();
        }
    }
}
