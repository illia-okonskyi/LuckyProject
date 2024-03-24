using LuckyProject.Lib.Basics.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IStringService
    {
        #region Extensions
        string NormalizeNewlines(string s);
        List<string> SplitByLines(string s);
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
    }
}
