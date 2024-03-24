using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LuckyProject.Lib.Basics.Services
{
    public class StringService : IStringService
    {
        #region Extensions
        public string NormalizeNewlines(string s) => s.ToNormalizedNewlines();
        public List<string> SplitByLines(string s) => s.SplitByLines();
        public string Surround(string s, string begin, string end) => s.ToSurrounded(begin, end);
        public string Surround(string s, string wrap) => s.ToSurrounded(wrap);
        public string Surround(string s, CommonStringSurround surround) => s.ToSurrounded(surround);
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
    }
}
