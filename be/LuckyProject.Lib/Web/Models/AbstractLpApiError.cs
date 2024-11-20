using Newtonsoft.Json.Converters;
using System;

namespace LuckyProject.Lib.Web.Models
{
    public abstract class AbstractLpApiError
    {
        public string Type { get; init; }
        public bool IsClientError { get; init; }

        protected AbstractLpApiError(string type, bool isClientError)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type = type;
            IsClientError = isClientError;
        }
    }
}
