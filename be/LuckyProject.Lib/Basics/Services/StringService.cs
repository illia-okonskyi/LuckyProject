using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LuckyProject.Lib.Basics.Services
{
    public class StringService : IStringService
    {
        #region Internals
        private readonly IEnvironmentService environmentService;
        #endregion

        #region ctor
        public StringService(IEnvironmentService environmentService)
        {
            this.environmentService = environmentService;
        }
        #endregion

        #region Newlines
        private static readonly Regex NewlineRegex = new(@"\r?\n");
        public string NormalizeNewlines(string s) =>
            NewlineRegex.Replace(s, environmentService.NewLine);
        public List<string> SplitByLines(string s) => NewlineRegex.Split(s).ToList();
        #endregion

        #region Surround
        public string Surround(string s, string begin, string end)
        {
            var sb = new StringBuilder(begin);
            sb.Append(s);
            sb.Append(end);
            return sb.ToString();
        }

        public string Surround(string s, string wrap)
        {
            return Surround(s, wrap, wrap);
        }

        public string Surround(string s, CommonStringSurround surround)
        {
            (var begin, var end) = surround switch
            {
                CommonStringSurround.Quotes => ("\"", "\""),
                CommonStringSurround.SingleQuotes => ("'", "'"),
                CommonStringSurround.BackQuotes => ("`", "`"),
                CommonStringSurround.Brackets => ("(", ")"),
                CommonStringSurround.Braces => ("{", "}"),
                CommonStringSurround.SquareBrackets => ("[", "]"),
                CommonStringSurround.TriangleBrackets => ("<", ">"),
                _ => throw new ArgumentException($"Unexpeceted {nameof(CommonStringSurround)}")
            };

            return Surround(s, begin, end);
        }
        #endregion

        #region Encoding
        public Encoding DefaultEncoding => Encoding.Default;

        public Encoding GetEncoding(string name) => Encoding.GetEncoding(name);
        public Encoding GetEncoding(int codePage) => Encoding.GetEncoding(codePage);

        public byte[] GetBytes(Encoding e, string s) =>
            e.GetBytes(s);
        public byte[] GetBytes(Encoding e, string s, int index, int count) =>
            e.GetBytes(s, index, count);
        
        public string GetString(Encoding e, byte[] bytes) =>
            e.GetString(bytes);
        public string GetString(Encoding e, byte[] bytes, int index, int count) =>
            e.GetString(bytes, index, count);
        public string GetString(Encoding e, ReadOnlySpan<byte> bytes) =>
            e.GetString(bytes);
        public string GetString(Encoding e, in ReadOnlySequence<byte> bytes) =>
            e.GetString(bytes);
        #endregion

        #region MemoryStream
        public MemoryStream ToMemoryStream(string s, Encoding e)
        {
            return new MemoryStream(GetBytes(e, s));
        }
        
        public string FromMemoryStream(MemoryStream ms, Encoding e)
        {
            return GetString(e, ms.ToArray());
        }
        #endregion

        #region Base64
        public string ToBase64String(
            ReadOnlySpan<byte> bytes,
            Base64FormattingOptions options = Base64FormattingOptions.None) =>
            Convert.ToBase64String(bytes, options);
        public string ToBase64String(
            byte[] inArray,
            int offset,
            int length,
            Base64FormattingOptions options) =>
            Convert.ToBase64String(inArray, offset, length, options);
        public string ToBase64String(byte[] inArray) =>
            Convert.ToBase64String(inArray);
        public string ToBase64String(byte[] inArray, Base64FormattingOptions options) =>
            Convert.ToBase64String(inArray, options);
        public string ToBase64String(byte[] inArray, int offset, int length) =>
            Convert.ToBase64String(inArray, offset, length);
        public byte[] FromBase64String(string s) =>
            Convert.FromBase64String(s);
        #endregion

        #region CamelCase/DashedCase
        public string ToCamelCase(string s, bool pascalCase = true)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            var firstLetter = s.Substring(0, 1);
            var firstResultLetter = pascalCase
                ? firstLetter.ToUpper()
                : firstLetter.ToLower();
            return $"{firstResultLetter}{s.Substring(1).ToLower()}";
        }

        public string ToCamelCase(List<string> words, bool pascalCase = true)
        {
            if (words.SafeCount() == 0)
            {
                throw new ArgumentException("Must have values", nameof(words));
            }

            var firstWord = ToCamelCase(words[0], pascalCase);
            var tailWords = words.Skip(1).Select(s => ToCamelCase(s, true)).ToList();
            var sb = new StringBuilder(firstWord);
            foreach (var word in tailWords)
            {
                sb.Append(word);
            }
            return sb.ToString();
        }

        public string ToDashedCase(List<string> words)
        {
            if (words.SafeCount() == 0)
            {
                throw new ArgumentException("Must have values", nameof(words));
            }

            return string.Join("-", words.Select(s => s.ToLower()));
        }

        private static readonly Regex CamelCaseRegex = new(
            @"(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][a-z]+)",
            RegexOptions.Compiled);

        public List<string> SplitCamelCase(string s)
        {
            return CamelCaseRegex.Matches(s)
                .OfType<Match>()
                .Select(m => m.Value)
                .ToList();
        }
        #endregion

        #region Html/Url encode
        public string UrlEncode(string s, Encoding e = null) => 
            HttpUtility.UrlEncode(s, e ?? DefaultEncoding);
        public string UrlDecode(string s, Encoding e = null) =>
            HttpUtility.UrlDecode(s, e ?? DefaultEncoding);
        public string HtmlEncode(string s) => HttpUtility.HtmlEncode(s);
        public string HtmlDecode(string s) => HttpUtility.HtmlDecode(s);
        public Dictionary<string, List<string>> ParseQueryString(string s)
        {
            var d = HttpUtility.ParseQueryString(HtmlDecode(s));
            var r = new Dictionary<string, List<string>>();
            foreach (var key in d.AllKeys)
            {
                r.Add(key, d.GetValues(key).ToList());
            }
            return r;
        }
        #endregion

        #region Uri helpers
        public string GetUriSchemeAndAuthority(Uri u, bool withTrailingSlash)
        {
            var sb = new StringBuilder(u.Scheme);
            sb.Append("://");
            sb.Append(u.Authority);
            if (withTrailingSlash)
            {
                sb.Append('/');
            }
            return sb.ToString();

        }
        public Uri GetUriSchemeAndAuthority(Uri u)
        {
            return new Uri(GetUriSchemeAndAuthority(u, true));
        }

        public Dictionary<string, StringValues> GetQueryParams(string query) =>
            QueryHelpers.ParseQuery(query);
        #endregion

        #region Random string gen
        public string GenerateRandomString(string chars, int length)
        {
            if (string.IsNullOrEmpty(chars))
            {
                throw new ArgumentNullException(nameof(chars));
            }

            if (length < 1)
            {
                throw new ArgumentException(nameof(length));
            }

            var sb = new StringBuilder();
            var random = new Random();

            for (uint i = 0; i < length; ++i)
            {
                sb.Append(chars[random.Next(0, chars.Length - 1)]);
            }

            return sb.ToString();
        }
        #endregion
    }
}
