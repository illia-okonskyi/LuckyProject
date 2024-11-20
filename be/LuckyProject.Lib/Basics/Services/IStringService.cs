using LuckyProject.Lib.Basics.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IStringService
    {
        #region Newlines
        string NormalizeNewlines(string s);
        List<string> SplitByLines(string s);
        #endregion

        #region Surround
        string Surround(string s, string begin, string end);
        string Surround(string s, string wrap);
        string Surround(string s, CommonStringSurround surround);
        #endregion

        #region Encoding
        Encoding DefaultEncoding { get; }

        Encoding GetEncoding(string name);
        Encoding GetEncoding(int codePage);

        byte[] GetBytes(Encoding e, string s);
        byte[] GetBytes(Encoding e, string s, int index, int count);

        string GetString(Encoding e, byte[] bytes);
        string GetString(Encoding e, byte[] bytes, int index, int count);
        string GetString(Encoding e, ReadOnlySpan<byte> bytes);
        string GetString(Encoding e, in ReadOnlySequence<byte> bytes);
        #endregion

        #region MemoryStream
        MemoryStream ToMemoryStream(string s, Encoding e);
        string FromMemoryStream(MemoryStream ms, Encoding e);
        #endregion

        #region Base64
        string ToBase64String(
            ReadOnlySpan<byte> bytes,
            Base64FormattingOptions options = Base64FormattingOptions.None);
        string ToBase64String(
            byte[] inArray,
            int offset,
            int length,
            Base64FormattingOptions options);
        string ToBase64String(byte[] inArray);
        string ToBase64String(byte[] inArray, Base64FormattingOptions options);
        string ToBase64String(byte[] inArray, int offset, int length);
        byte[] FromBase64String(string s);
        #endregion

        #region CamelCase/DashedCase
        string ToCamelCase(string s, bool pascalCase = true);
        string ToCamelCase(List<string> words, bool pascalCase = true);
        string ToDashedCase(List<string> words);
        List<string> SplitCamelCase(string s);
        #endregion

        #region Html/Url encode
        string UrlEncode(string s, Encoding e = null);
        string UrlDecode(string s, Encoding e = null);
        string HtmlEncode(string s);
        string HtmlDecode(string s);
        Dictionary<string, List<string>> ParseQueryString(string s);
        #endregion

        #region Uri helpers
        string GetUriSchemeAndAuthority(Uri u, bool withTrailingSlash);
        Uri GetUriSchemeAndAuthority(Uri u);
        Dictionary<string, StringValues> GetQueryParams(string query);
        #endregion

        #region Random string gen
        string GenerateRandomString(string chars, int length);
        #endregion
    }
}
