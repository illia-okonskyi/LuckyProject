using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Web.Models;
using System.Net;

namespace LuckyProject.Lib.Web.Exceptions
{
    public class LpApiRequestException : LpException
    {
        public enum ErrorType
        {
            LpApiError,
            OtherError
        }

        public ErrorType Type { get; }
        public AbstractLpApiError LpApiError { get; }
        public HttpStatusCode StatusCode { get; }
        public string Content { get; }

        private LpApiRequestException(
            string message,
            ErrorType type,
            AbstractLpApiError lpApiError,
            HttpStatusCode statusCode,
            string content)
            : base(message)
        {
            Type = type;
            LpApiError = lpApiError;
            StatusCode = statusCode;
            Content = content;
        }

        public LpApiRequestException(AbstractLpApiError lpApiError)
            : this($"LpApiError {lpApiError.Type}",
                  ErrorType.LpApiError,
                  lpApiError,
                  HttpStatusCode.OK,
                  null)
        { }

        public LpApiRequestException(HttpStatusCode statusCode, string content = null)
            : this($"OtherError {statusCode}",
                  ErrorType.OtherError,
                  null,
                  statusCode,
                  content)
        { }
    }
}
