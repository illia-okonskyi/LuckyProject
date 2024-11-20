using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.Services
{
    public class LpDefaultValidationLocalization : ILpDefaultValidationLocalization
    {
        #region KeysAndDefaults
        private static readonly Dictionary<string, string> KeysAndDefaults = new()
        {
            {
                "lp.lib.winui.common.strings.validation.notNull",
                "Must have value"
            },
            {
                "lp.lib.winui.common.strings.validation.null",
                "Must not have value"
            },
            {
                "lp.lib.winui.common.strings.validation.notEmpty",
                "Must have value"
            },
            {
                "lp.lib.winui.common.strings.validation.empty",
                "Must not have value"
            },
            {
                "lp.lib.winui.common.strings.validation.notEqual",
                "Must be not equal equal to {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.equal",
                "Must be equal to {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.length",
                "Length must be from {0} to {1}"
            },
            {
                "lp.lib.winui.common.strings.validation.minLength",
                "Length must be not less than {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.maxLength",
                "Length must be not greater than {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.lessThan",
                "Must be not less than {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.lessThanOrEqual",
                "Must be not less than or equal to {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.greaterThan",
                "Must be not greater than {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.greaterThanOrEqual",
                "Must be not greater than or equal to {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.regex",
                "Must match regular expression \\{0}\\"
            },
            {
                "lp.lib.winui.common.strings.validation.email",
                "Must be valid email"
            },
            {
                "lp.lib.winui.common.strings.validation.login",
                "Must be valid login"
            },
            {
                "lp.lib.winui.common.strings.validation.password",
                "Must follow password security rules: {0}"
            },
            {
                "lp.lib.winui.common.strings.validation.credentials",
                "Invalid credentials"
            },
            {
                "lp.lib.winui.common.strings.validation.url",
                "Must be URL that follow the {0} rule"
            },
            {
                "lp.lib.winui.common.strings.validation.fullName",
                "Invalid International Person Full Name"
            },
            {
                "lp.lib.winui.common.strings.validation.permission",
                "Invalid Permission"
            },
            {
                "lp.lib.winui.common.strings.validation.phone",
                "Must be Phone Number in International format with leading + sign"
            },
            {
                "lp.lib.winui.common.strings.validation.creditCard",
                "Invalid credit card number"
            },
            {
                "lp.lib.winui.common.strings.validation.enum",
                "Must be one of the enumeration/listed values"
            },
            {
                "lp.lib.winui.common.strings.validation.exclusiveBetween",
                "Must be greater than {0} and less than {1}"
            },
            {
                "lp.lib.winui.common.strings.validation.inclusiveBetween",
                "Must be greater than or equal to {0} and less than or equal to {1}"
            },
            {
                "lp.lib.winui.common.strings.validation.precisionScale",
                "Must not be more than {0} digits in total, with allowance for {1} decimals"
            },
            {
                "lp.lib.winui.common.strings.validation.unique",
                "Must be unique (none of existing)"
            },
            {
                "lp.lib.winui.common.strings.validation.notUnique",
                "Must be not unique (one of existing)"
            },
            {
                "lp.lib.winui.common.strings.validation.notFound",
                "Specified item not found"
            },
            {
                "lp.lib.winui.common.strings.validation.accessDenied",
                "Access denied"
            },
            {
                "lp.lib.winui.common.strings.validation.prefixed",
                "Must start with \"{0}\""
            },
            {
                "lp.lib.winui.common.strings.validation.notPrefixed",
                "Must not start with \"{0}\""
            },
            {
                "lp.lib.winui.common.strings.validation.suffixed",
                "Must end with \"{0}\""
            },
            {
                "lp.lib.winui.common.strings.validation.notSuffixed",
                "Must not end with \"{0}\""
            },
            {
                "lp.lib.winui.common.strings.validation.unknown",
                "Validation rule not specified, but value is not valid"
            },
        };
        #endregion

        #region Interface impl
        #region Localized values & error formatters
        public string NotNull { get; private set; }
        public string Null { get; private set; }
        public string NotEmpty { get; private set; }
        public string Empty { get; private set; }
        public string NotEqual { get; private set; }
        public string FormatNotEqual(LpValidationError error) =>
            string.Format(NotEqual, error.Details[ValidationErrorCodes.Details.Target]);
        public string Equal { get; private set; }
        public string FormatEqual(LpValidationError error) =>
            string.Format(Equal, error.Details[ValidationErrorCodes.Details.Target]);
        public string Length { get; private set; }
        public string FormatLength(LpValidationError error) =>
            string.Format(Length,
                error.Details[ValidationErrorCodes.Details.Min],
                error.Details[ValidationErrorCodes.Details.Max]);
        public string MinLength { get; private set; }
        public string FormatMinLength(LpValidationError error) =>
            string.Format(MinLength, error.Details[ValidationErrorCodes.Details.Target]);
        public string MaxLength { get; private set; }
        public string FormatMaxLength(LpValidationError error) =>
            string.Format(MaxLength, error.Details[ValidationErrorCodes.Details.Target]);
        public string LessThan { get; private set; }
        public string FormatLessThan(LpValidationError error) =>
            string.Format(LessThan, error.Details[ValidationErrorCodes.Details.Target]);
        public string LessThanOrEqual { get; private set; }
        public string FormatLessThanOrEqual(LpValidationError error) =>
            string.Format(LessThanOrEqual, error.Details[ValidationErrorCodes.Details.Target]);
        public string GreaterThan { get; private set; }
        public string FormatGreaterThan(LpValidationError error) =>
            string.Format(GreaterThan, error.Details[ValidationErrorCodes.Details.Target]);
        public string GreaterThanOrEqual { get; private set; }
        public string FormatGreaterThanOrEqual(LpValidationError error) =>
            string.Format(GreaterThanOrEqual, error.Details[ValidationErrorCodes.Details.Target]);
        public string Regex { get; private set; }
        public string FormatRegex(LpValidationError error) =>
            string.Format(Regex, error.Details[ValidationErrorCodes.Details.Target]);
        public string Email { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Credentials { get; private set; }
        public string Url { get; private set; }
        public string FormatUrl(LpValidationError error) =>
            string.Format(Url, error.Details[ValidationErrorCodes.Details.Target]);
        public string FullName { get; private set; }
        public string Permission { get; private set; }
        public string Phone { get; private set; }
        public string CreditCard { get; private set; }
        public string Enum { get; private set; }
        public string ExclusiveBetween { get; private set; }
        public string FormatExclusiveBetween(LpValidationError error) =>
            string.Format(ExclusiveBetween,
                error.Details[ValidationErrorCodes.Details.Min],
                error.Details[ValidationErrorCodes.Details.Max]);
        public string InclusiveBetween { get; private set; }
        public string FormatInclusiveBetween(LpValidationError error) =>
            string.Format(InclusiveBetween,
                error.Details[ValidationErrorCodes.Details.Min],
                error.Details[ValidationErrorCodes.Details.Max]);
        public string PrecisionScale { get; private set; }
        public string FormatPrecisionScale(LpValidationError error) =>
            string.Format(ExclusiveBetween,
                error.Details[ValidationErrorCodes.Details.Precision],
                error.Details[ValidationErrorCodes.Details.Scale]);
        public string Unique { get; private set; }
        public string NotUnique { get; private set; }
        public string FormatUnique(LpValidationError error)
        {
            var mustBeUnique = bool.Parse(ValidationErrorCodes.Details.Target);
            return mustBeUnique ? Unique : NotUnique;
        }
        public string NotFound { get; private set; }
        public string AccessDenied { get; private set; }
        public string Prefixed { get; private set; }
        public string NotPrefixed { get; private set; }
        public string FormatPrefix(LpValidationError error)
        {
            var mustBePrefixed = bool.Parse(ValidationErrorCodes.Details.Target2);
            return string.Format(
                mustBePrefixed ? Prefixed : NotPrefixed,
                error.Details[ValidationErrorCodes.Details.Target]);
        }
        public string Suffixed { get; private set; }
        public string NotSuffixed { get; private set; }
        public string FormatSuffix(LpValidationError error)
        {
            var mustBeSuffixed = bool.Parse(ValidationErrorCodes.Details.Target2);
            return string.Format(
                mustBeSuffixed ? Suffixed : NotSuffixed,
                error.Details[ValidationErrorCodes.Details.Target]);
        }
        public string Unknown { get; private set; }
        #endregion

        #region GetMessages
        public List<string> GetMessages(LpValidationError error)
        {
            var code = error.Code;
            if (code == ValidationErrorCodes.MessagesOnly)
            {
                return error.Messages;
            }

            var message = code switch
            {
                ValidationErrorCodes.NotNull => NotNull,
                ValidationErrorCodes.Null => Null,
                ValidationErrorCodes.NotEmpty => NotEmpty,
                ValidationErrorCodes.Empty => Empty,
                ValidationErrorCodes.NotEqual => FormatNotEqual(error),
                ValidationErrorCodes.Equal => FormatEqual(error),
                ValidationErrorCodes.Length => FormatLength(error),
                ValidationErrorCodes.MinLength => FormatMinLength(error),
                ValidationErrorCodes.MaxLength => FormatMaxLength(error),
                ValidationErrorCodes.LessThan => FormatLessThan(error),
                ValidationErrorCodes.LessThanOrEqual => FormatLessThanOrEqual(error),
                ValidationErrorCodes.GreaterThan => FormatGreaterThan(error),
                ValidationErrorCodes.GreaterThanOrEqual => FormatGreaterThanOrEqual(error),
                ValidationErrorCodes.Regex => FormatRegex(error),
                ValidationErrorCodes.Email => Email,
                ValidationErrorCodes.Login => Login,
                ValidationErrorCodes.Password => Password,
                ValidationErrorCodes.Credentials => Credentials,
                ValidationErrorCodes.Url => FormatUrl(error),
                ValidationErrorCodes.FullName => FullName,
                ValidationErrorCodes.Permission => Permission,
                ValidationErrorCodes.Phone => Phone,
                ValidationErrorCodes.CreditCard => CreditCard,
                ValidationErrorCodes.Enum => Enum,
                ValidationErrorCodes.ExclusiveBetween => FormatExclusiveBetween(error),
                ValidationErrorCodes.InclusiveBetween => FormatInclusiveBetween(error),
                ValidationErrorCodes.PrecisionScale => FormatPrecisionScale(error),
                ValidationErrorCodes.Unique => FormatUnique(error),
                ValidationErrorCodes.NotFound => NotFound,
                ValidationErrorCodes.AccessDenied => AccessDenied,
                ValidationErrorCodes.Prefix => FormatPrefix(error),
                ValidationErrorCodes.Suffix => FormatSuffix(error),
                ValidationErrorCodes.Unknown => Unknown,
                _ => null,
            };

            return message != null ? [message] : error.Messages;
        }
        #endregion
        #endregion

        #region Internal usage
        public void Update(ILpDesktopAppLocalizationService service)
        {
            var localization = service.GetLocalizedStrings(KeysAndDefaults);
            NotNull = localization["lp.lib.winui.common.strings.validation.notNull"];
            Null = localization["lp.lib.winui.common.strings.validation.null"];
            NotEmpty = localization["lp.lib.winui.common.strings.validation.notEmpty"];
            Empty = localization["lp.lib.winui.common.strings.validation.empty"];
            NotEqual = localization["lp.lib.winui.common.strings.validation.notEqual"];
            Equal = localization["lp.lib.winui.common.strings.validation.equal"];
            Length = localization["lp.lib.winui.common.strings.validation.length"];
            MinLength = localization["lp.lib.winui.common.strings.validation.minLength"];
            MaxLength = localization["lp.lib.winui.common.strings.validation.maxLength"];
            LessThan = localization["lp.lib.winui.common.strings.validation.lessThan"];
            LessThanOrEqual = localization["lp.lib.winui.common.strings.validation.lessThanOrEqual"];
            GreaterThan = localization["lp.lib.winui.common.strings.validation.greaterThan"];
            GreaterThanOrEqual =
                localization["lp.lib.winui.common.strings.validation.greaterThanOrEqual"];
            Regex = localization["lp.lib.winui.common.strings.validation.regex"];
            Email = localization["lp.lib.winui.common.strings.validation.email"];
            Login = localization["lp.lib.winui.common.strings.validation.login"];
            Password = localization["lp.lib.winui.common.strings.validation.password"];
            Credentials = localization["lp.lib.winui.common.strings.validation.credentials"];
            Url = localization["lp.lib.winui.common.strings.validation.url"];
            FullName = localization["lp.lib.winui.common.strings.validation.fullName"];
            Permission = localization["lp.lib.winui.common.strings.validation.permission"];
            Phone = localization["lp.lib.winui.common.strings.validation.phone"];
            CreditCard = localization["lp.lib.winui.common.strings.validation.creditCard"];
            Enum = localization["lp.lib.winui.common.strings.validation.enum"];
            ExclusiveBetween =
                localization["lp.lib.winui.common.strings.validation.exclusiveBetween"];
            InclusiveBetween =
                localization["lp.lib.winui.common.strings.validation.inclusiveBetween"];
            PrecisionScale = localization["lp.lib.winui.common.strings.validation.precisionScale"];
            Unique = localization["lp.lib.winui.common.strings.validation.unique"];
            NotUnique = localization["lp.lib.winui.common.strings.validation.notUnique"];
            NotFound = localization["lp.lib.winui.common.strings.validation.notFound"];
            AccessDenied = localization["lp.lib.winui.common.strings.validation.accessDenied"];
            Prefixed = localization["lp.lib.winui.common.strings.validation.prefixed"];
            NotPrefixed = localization["lp.lib.winui.common.strings.validation.notPrefixed"];
            Suffixed = localization["lp.lib.winui.common.strings.validation.suffixed"];
            NotSuffixed = localization["lp.lib.winui.common.strings.validation.notSuffixed"];
            Unknown = localization["lp.lib.winui.common.strings.validation.unknown"];
        }
        #endregion
    }
}
