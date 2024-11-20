using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.Services
{
    public interface ILpDefaultValidationLocalization
    {
        #region Localized values & error formatters
        string NotNull { get; }
        string Null { get; }
        string NotEmpty { get; }
        string Empty { get; }
        string NotEqual { get; }
        string FormatNotEqual(LpValidationError error);
        string Equal { get; }
        string FormatEqual(LpValidationError error);
        string Length { get; }
        string FormatLength(LpValidationError error);
        string MinLength { get; }
        string FormatMinLength(LpValidationError error);
        string MaxLength { get; }
        string FormatMaxLength(LpValidationError error);
        string LessThan { get; }
        string FormatLessThan(LpValidationError error);
        string LessThanOrEqual { get; }
        string FormatLessThanOrEqual(LpValidationError error);
        string GreaterThan { get; }
        string FormatGreaterThan(LpValidationError error);
        string GreaterThanOrEqual { get; }
        string FormatGreaterThanOrEqual(LpValidationError error);
        string Regex { get; }
        string FormatRegex(LpValidationError error);
        string Email { get; }
        string Login { get; }
        string Password { get; }
        string Credentials { get; }
        string Url { get; }
        string FormatUrl(LpValidationError error);
        string FullName { get; }
        string Permission { get; }
        string Phone { get; }
        string CreditCard { get; }
        string Enum { get; }
        string ExclusiveBetween { get; }
        string FormatExclusiveBetween(LpValidationError error);
        string InclusiveBetween { get; }
        string FormatInclusiveBetween(LpValidationError error);
        string PrecisionScale { get; }
        string FormatPrecisionScale(LpValidationError error);
        string Unique { get; }
        string NotUnique { get; }
        string FormatUnique(LpValidationError error);
        string NotFound { get; }
        string AccessDenied { get; }
        string Prefixed { get; }
        string NotPrefixed { get; }
        string FormatPrefix(LpValidationError error);
        string Suffixed { get; }
        string NotSuffixed { get; }
        string FormatSuffix(LpValidationError error);
        string Unknown { get; }
        #endregion

        #region GetMessages
        List<string> GetMessages(LpValidationError error);
        #endregion
    }
}
