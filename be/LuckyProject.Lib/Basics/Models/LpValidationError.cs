using LuckyProject.Lib.Basics.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Basics.Models
{
    public class LpValidationError
    {
        #region Properties
        public string Path { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public List<string> Messages { get; init; } = new();

        [JsonIgnore]
        public string SingleMessage => string.Join("; ", Messages);

        public Dictionary<string, string> Details { get; init; } = new();
        #endregion

        #region ctors
        public LpValidationError()
        { }

        public LpValidationError(
            string path,
            string code,
            List<string> messages = null,
            Dictionary<string, string> details = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            Path = path ?? string.Empty;
            Code = code;
            if (messages != null)
            {
                Messages.AddRange(messages);
            }
            if (details != null)
            {
                Details = details;
            }
        }

        public LpValidationError(
             string path,
             string code,
             string message = null,
             Dictionary<string, string> details = null)
            : this(path, code, message != null ? new List<string> { message } : null, details)
        { }

        public LpValidationError(
            string path,
            List<string> messages)
            : this(path, ValidationErrorCodes.MessagesOnly, messages, null)
        { }

        public LpValidationError(
            string path,
            string message)
            : this(path, new List<string> { message })
        { }
        #endregion

        public override string ToString()
        {
            var details = string.Join(
                "; ",
                Details.Select(kvp => $"{kvp.Key} = {kvp.Value}").ToList());
            return $"LpValidationError({Path}/{Code}): {SingleMessage}; [{details}]";
        }
    }
}
