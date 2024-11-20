using LuckyProject.Lib.Basics.Models;
using System.Text;

namespace LuckyProject.Lib.Basics.Exceptions
{
    public class LpValidationErrorException : LpException
    {
        public LpValidationResult Result { get; }

        public LpValidationErrorException(LpValidationResult result)
            : base(BuildMessage(result))
        {
            Result = result;
        }

        private static string BuildMessage(LpValidationResult result)
        {
            if (result.IsCancelled)
            {
                return "ValidationError: Validation cancelled";
            }

            var sb = new StringBuilder("ValidationError: (");
            foreach(var error in result.Errors)
            {
                sb.Append(error.Path);
                sb.Append("/");
                sb.Append(error.Code);
                if (error.Details.Count > 0)
                {
                    sb.Append("/{");
                    foreach(var detail in error.Details)
                    {
                        sb.Append(detail.Key);
                        sb.Append("=");
                        sb.Append(detail.Value);
                    }
                    sb.Append("}");
                }
                sb.Append("; ");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
