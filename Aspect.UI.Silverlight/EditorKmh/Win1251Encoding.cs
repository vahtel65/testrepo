﻿/*-----------------------------------------------------------------------------/
    This code was generated by a tool: http://enc.drachev.com/
    You can get a fresh version any time you want.

    Changes to this file may cause incorrect behavior and will be lost if
    the code is regenerated.
 
    Anton Drachev, as the author of this code, expressly permits 
    you to use this code as public domain code.

    Supported codepages: 
        1200
        1201
        1250
        20127
        65001
    Generated: Tue, 25 Oct 2011 17:13:51 GMT
/-----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace EditorKmh 
{
    public abstract class Encoding
    {
        #region toString

        public virtual char[] GetChars(byte[] bytes)
        {
            return GetChars(bytes, 0, bytes.Length);
        }

        public abstract char[] GetChars(byte[] bytes, int index, int count);

        public virtual int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var tmp = GetChars(bytes, byteIndex, byteCount);
            Array.Copy(tmp, 0, chars, charIndex, tmp.Length);
            return tmp.Length;
        }

        public virtual string GetString(byte[] bytes)
        {
            return GetString(bytes, 0, bytes.Length);
        }

        public virtual string GetString(byte[] bytes, int start, int count)
        {
            return new string(GetChars(bytes, start, count));
        }

        public virtual byte[] GetBytes(char[] chars)
        {
            return GetBytes(chars, 0, chars.Length);
        }

        #endregion

        #region toBytes

        public virtual byte[] GetBytes(string s)
        {
            var tmp = s.ToCharArray();
            return GetBytes(tmp, 0, tmp.Length);
        }

        public abstract byte[] GetBytes(char[] chars, int index, int count);

        public virtual int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var tmp = GetBytes(chars, charIndex, charCount);
            Array.Copy(tmp, 0, bytes, byteIndex, tmp.Length);
            return tmp.Length;
        }

        public virtual int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return GetBytes(s.ToCharArray(), charIndex, charCount, bytes, byteIndex);
        }

        #endregion

        #region counts

        public virtual int GetByteCount(char[] chars)
        {
            return GetByteCount(chars, 0, chars.Length);
        }

        public virtual int GetByteCount(String s)
        {
            var tmp = s.ToCharArray();
            return GetByteCount(tmp, 0, tmp.Length);
        }

        public abstract int GetByteCount(char[] chars, int index, int count);

        public virtual int GetCharCount(byte[] bytes)
        {
            return GetCharCount(bytes, 0, bytes.Length);
        }

        public abstract int GetCharCount(byte[] bytes, int index, int count);

        #endregion

        #region names

        public abstract int CodePage { get; }
        public abstract string EncodingName { get; }
        public abstract string WebName { get; }

        private static readonly object syncRoot = new object();

        private static Dictionary<string, int> NameMap;

        public static Encoding GetEncoding(string name)
        {
            if (NameMap == null)
                lock (syncRoot)
                    if (NameMap == null)
                        NameMap = new Dictionary<string, int> {
                            { "unicode", 1200 },
                            { "utf-16", 1200 },
                            { "cp1200", 1200 },
                            { "ucs-2", 1200 },
                            { "utf-16le", 1200 },
                            { "unicodefffe", 1201 },
                            { "utf-16be", 1201 },
                            { "cp1201", 1201 },
                            { "windows-1250", 1250 },
                            { "x-cp1250", 1250 },
                            { "cp1250", 1250 },
                            { "us-ascii", 20127 },
                            { "ansi_x3.4-1968", 20127 },
                            { "ansi_x3.4-1986", 20127 },
                            { "ascii", 20127 },
                            { "cp367", 20127 },
                            { "csascii", 20127 },
                            { "ibm367", 20127 },
                            { "iso_646.irv:1991", 20127 },
                            { "iso646-us", 20127 },
                            { "iso-ir-6", 20127 },
                            { "us", 20127 },
                            { "utf-8", 65001 },
                            { "unicode-1-1-utf-8", 65001 },
                            { "unicode-2-0-utf-8", 65001 },
                            { "x-unicode-2-0-utf-8", 65001 },
                            { "cp65001", 65001 },
                        };
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (NameMap.ContainsKey(name))
                return GetEncoding(NameMap[name]);
            throw new ArgumentException("'" + name + "' is not a supported encoding name.", "name");
        }

        public static Encoding GetEncoding(int codepage)
        {
            switch (codepage)
            {
                case 0: return Default;
                case 1200: return Unicode;
                case 1201: return BigEndianUnicode;
                case 65001: return UTF8;
                case 20127: return ASCII;
                case 1250: return Enc1250.Value ?? (Enc1250.Value = new Enc1250());
                default:
                    if (0 <= codepage && codepage <= 65535)
                        throw new NotSupportedException("No data is available for encoding " + codepage + ".");
                    throw new ArgumentException("Valid values are between 0 and 65535, inclusive.", "codepage");
            }
        }

        #endregion

        public static readonly Encoding ASCII = new Enc20127();
        public static readonly Encoding BigEndianUnicode = new EncodingWrapper(System.Text.Encoding.BigEndianUnicode);
        public static readonly Encoding Default = UTF8;
        public static readonly Encoding Unicode = new EncodingWrapper(System.Text.Encoding.Unicode);
        public static readonly Encoding UTF8 = new EncodingWrapper(System.Text.Encoding.UTF8);

        private sealed class EncodingWrapper : Encoding
        {
            readonly System.Text.Encoding enc;

            public EncodingWrapper(System.Text.Encoding enc)
            {
                this.enc = enc;
            }

            public override int GetByteCount(char[] chars)
            {
                return enc.GetByteCount(chars);
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                return enc.GetByteCount(chars, index, count);
            }

            public override int GetByteCount(string s)
            {
                return enc.GetByteCount(s);
            }

            public override byte[] GetBytes(char[] chars)
            {
                return enc.GetBytes(chars);
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                return enc.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
            }

            public override byte[] GetBytes(char[] chars, int index, int count)
            {
                return enc.GetBytes(chars, index, count);
            }

            public override byte[] GetBytes(string s)
            {
                return enc.GetBytes(s);
            }

            public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                return enc.GetBytes(s, charIndex, charCount, bytes, byteIndex);
            }

            public override int GetCharCount(byte[] bytes)
            {
                return enc.GetCharCount(bytes);
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return enc.GetCharCount(bytes, index, count);
            }

            public override char[] GetChars(byte[] bytes)
            {
                return enc.GetChars(bytes);
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                return enc.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            }

            public override char[] GetChars(byte[] bytes, int index, int count)
            {
                return enc.GetChars(bytes, index, count);
            }

            public override string GetString(byte[] bytes, int start, int count)
            {
                return enc.GetString(bytes, start, count);
            }

            public override string WebName
            {
                get { return enc.WebName; }
            }

            public override int CodePage
            {
                get
                {
                    switch (WebName)
                    {
                        case "utf-16BE":
                        case "unicodeFFFE": return 1201;
                        case "utf-16": return 1200;
                        case "utf-8": return 65001;
                        default: return -1;
                    }
                }
            }

            public override string EncodingName
            {
                get
                {
                    switch (WebName)
                    {
                        case "utf-16BE":
                        case "unicodeFFFE": return "Unicode (Big-Endian)";
                        case "utf-16": return "Unicode";
                        case "utf-8": return "Unicode (UTF-8)";
                        default: return "(Unknown)";
                    }
                }
            }
        }

        // Central European (Windows)
        private sealed class Enc1250 : Encoding
        {
            public static Enc1250 Value;

            #region conversion

            private static char[] charMap = new[] { 
                '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', 
                '\u0008', '\u0009', '\u000A', '\u000B', '\u000C', '\u000D', '\u000E', '\u000F', 
                '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
                '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F', 
                '\u0020', '!', '\u0022', '#', '$', '%', '&', '\u0027', 
                '(', ')', '*', '+', ',', '\u002D', '.', '/', 
                '0', '1', '2', '3', '4', '5', '6', '7', 
                '8', '9', ':', ';', '<', '=', '>', '?', 
                '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 
                'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 
                'X', 'Y', 'Z', '\u005B', '\u005C', '\u005D', '^', '_', 
                '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 
                'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 
                'x', 'y', 'z', '{', '|', '}', '~', '\u007F', 
                '\u20AC', '\u0081', '\u201A', '\u0083', '\u201E', '\u2026', '\u2020', '\u2021', 
                '\u0088', '\u2030', '\u0160', '\u2039', '\u015A', '\u0164', '\u017D', '\u0179', 
                '\u0090', '\u2018', '\u2019', '\u201C', '\u201D', '\u2022', '\u2013', '\u2014', 
                '\u0098', '\u2122', '\u0161', '\u203A', '\u015B', '\u0165', '\u017E', '\u017A', 
                '\u00A0', '\u02C7', '\u02D8', '\u0141', '\u00A4', '\u0104', '\u00A6', '\u00A7', 
                '\u00A8', '\u00A9', '\u015E', '\u00AB', '\u00AC', '\u00AD', '\u00AE', '\u017B', 
                '\u00B0', '\u00B1', '\u02DB', '\u0142', '\u00B4', '\u00B5', '\u00B6', '\u00B7', 
                '\u00B8', '\u0105', '\u015F', '\u00BB', '\u013D', '\u02DD', '\u013E', '\u017C', 
                '\u0154', '\u00C1', '\u00C2', '\u0102', '\u00C4', '\u0139', '\u0106', '\u00C7', 
                '\u010C', '\u00C9', '\u0118', '\u00CB', '\u011A', '\u00CD', '\u00CE', '\u010E', 
                '\u0110', '\u0143', '\u0147', '\u00D3', '\u00D4', '\u0150', '\u00D6', '\u00D7', 
                '\u0158', '\u016E', '\u00DA', '\u0170', '\u00DC', '\u00DD', '\u0162', '\u00DF', 
                '\u0155', '\u00E1', '\u00E2', '\u0103', '\u00E4', '\u013A', '\u0107', '\u00E7', 
                '\u010D', '\u00E9', '\u0119', '\u00EB', '\u011B', '\u00ED', '\u00EE', '\u010F', 
                '\u0111', '\u0144', '\u0148', '\u00F3', '\u00F4', '\u0151', '\u00F6', '\u00F7', 
                '\u0159', '\u016F', '\u00FA', '\u0171', '\u00FC', '\u00FD', '\u0163', '\u02D9' };

            public override char[] GetChars(byte[] bytes, int index, int count)
            {
                var result = new char[count];
                for (var i = 0; i < result.Length; i++)
                    result[i] = charMap[bytes[i + index]];
                return result;
            }

            public override byte[] GetBytes(char[] chars, int index, int count)
            {
                var result = new byte[count];
                for (var i = 0; i < result.Length; i++)
                {
                    var c = chars[i + index];
                    if (c < 128)
                    {
                        result[i] = (byte)c;
                        continue;
                    }
                    switch (c)
                    {
                        case '\u20AC': result[i] = 128; break;
                        case '\u0081': result[i] = 129; break;
                        case '\u201A': result[i] = 130; break;
                        case '\u0083': result[i] = 131; break;
                        case '\u201E': result[i] = 132; break;
                        case '\u2026': result[i] = 133; break;
                        case '\u2020': result[i] = 134; break;
                        case '\u2021': result[i] = 135; break;
                        case '\u0088': result[i] = 136; break;
                        case '\u2030': result[i] = 137; break;
                        case '\u0160': result[i] = 138; break;
                        case '\u2039': result[i] = 139; break;
                        case '\u015A': result[i] = 140; break;
                        case '\u0164': result[i] = 141; break;
                        case '\u017D': result[i] = 142; break;
                        case '\u0179': result[i] = 143; break;
                        case '\u0090': result[i] = 144; break;
                        case '\u2018': result[i] = 145; break;
                        case '\u2019': result[i] = 146; break;
                        case '\u201C': result[i] = 147; break;
                        case '\u201D': result[i] = 148; break;
                        case '\u2022': result[i] = 149; break;
                        case '\u2013': result[i] = 150; break;
                        case '\u2014': result[i] = 151; break;
                        case '\u0098': result[i] = 152; break;
                        case '\u2122': result[i] = 153; break;
                        case '\u0161': result[i] = 154; break;
                        case '\u203A': result[i] = 155; break;
                        case '\u015B': result[i] = 156; break;
                        case '\u0165': result[i] = 157; break;
                        case '\u017E': result[i] = 158; break;
                        case '\u017A': result[i] = 159; break;
                        case '\u00A0': result[i] = 160; break;
                        case '\u02C7': result[i] = 161; break;
                        case '\u02D8': result[i] = 162; break;
                        case '\u0141': result[i] = 163; break;
                        case '\u00A4': result[i] = 164; break;
                        case '\u0104': result[i] = 165; break;
                        case '\u00A6': result[i] = 166; break;
                        case '\u00A7': result[i] = 167; break;
                        case '\u00A8': result[i] = 168; break;
                        case '\u00A9': result[i] = 169; break;
                        case '\u015E': result[i] = 170; break;
                        case '\u00AB': result[i] = 171; break;
                        case '\u00AC': result[i] = 172; break;
                        case '\u00AD': result[i] = 173; break;
                        case '\u00AE': result[i] = 174; break;
                        case '\u017B': result[i] = 175; break;
                        case '\u00B0': result[i] = 176; break;
                        case '\u00B1': result[i] = 177; break;
                        case '\u02DB': result[i] = 178; break;
                        case '\u0142': result[i] = 179; break;
                        case '\u00B4': result[i] = 180; break;
                        case '\u00B5': result[i] = 181; break;
                        case '\u00B6': result[i] = 182; break;
                        case '\u00B7': result[i] = 183; break;
                        case '\u00B8': result[i] = 184; break;
                        case '\u0105': result[i] = 185; break;
                        case '\u015F': result[i] = 186; break;
                        case '\u00BB': result[i] = 187; break;
                        case '\u013D': result[i] = 188; break;
                        case '\u02DD': result[i] = 189; break;
                        case '\u013E': result[i] = 190; break;
                        case '\u017C': result[i] = 191; break;
                        case '\u0154': result[i] = 192; break;
                        case '\u00C1': result[i] = 193; break;
                        case '\u00C2': result[i] = 194; break;
                        case '\u0102': result[i] = 195; break;
                        case '\u00C4': result[i] = 196; break;
                        case '\u0139': result[i] = 197; break;
                        case '\u0106': result[i] = 198; break;
                        case '\u00C7': result[i] = 199; break;
                        case '\u010C': result[i] = 200; break;
                        case '\u00C9': result[i] = 201; break;
                        case '\u0118': result[i] = 202; break;
                        case '\u00CB': result[i] = 203; break;
                        case '\u011A': result[i] = 204; break;
                        case '\u00CD': result[i] = 205; break;
                        case '\u00CE': result[i] = 206; break;
                        case '\u010E': result[i] = 207; break;
                        case '\u0110': result[i] = 208; break;
                        case '\u0143': result[i] = 209; break;
                        case '\u0147': result[i] = 210; break;
                        case '\u00D3': result[i] = 211; break;
                        case '\u00D4': result[i] = 212; break;
                        case '\u0150': result[i] = 213; break;
                        case '\u00D6': result[i] = 214; break;
                        case '\u00D7': result[i] = 215; break;
                        case '\u0158': result[i] = 216; break;
                        case '\u016E': result[i] = 217; break;
                        case '\u00DA': result[i] = 218; break;
                        case '\u0170': result[i] = 219; break;
                        case '\u00DC': result[i] = 220; break;
                        case '\u00DD': result[i] = 221; break;
                        case '\u0162': result[i] = 222; break;
                        case '\u00DF': result[i] = 223; break;
                        case '\u0155': result[i] = 224; break;
                        case '\u00E1': result[i] = 225; break;
                        case '\u00E2': result[i] = 226; break;
                        case '\u0103': result[i] = 227; break;
                        case '\u00E4': result[i] = 228; break;
                        case '\u013A': result[i] = 229; break;
                        case '\u0107': result[i] = 230; break;
                        case '\u00E7': result[i] = 231; break;
                        case '\u010D': result[i] = 232; break;
                        case '\u00E9': result[i] = 233; break;
                        case '\u0119': result[i] = 234; break;
                        case '\u00EB': result[i] = 235; break;
                        case '\u011B': result[i] = 236; break;
                        case '\u00ED': result[i] = 237; break;
                        case '\u00EE': result[i] = 238; break;
                        case '\u010F': result[i] = 239; break;
                        case '\u0111': result[i] = 240; break;
                        case '\u0144': result[i] = 241; break;
                        case '\u0148': result[i] = 242; break;
                        case '\u00F3': result[i] = 243; break;
                        case '\u00F4': result[i] = 244; break;
                        case '\u0151': result[i] = 245; break;
                        case '\u00F6': result[i] = 246; break;
                        case '\u00F7': result[i] = 247; break;
                        case '\u0159': result[i] = 248; break;
                        case '\u016F': result[i] = 249; break;
                        case '\u00FA': result[i] = 250; break;
                        case '\u0171': result[i] = 251; break;
                        case '\u00FC': result[i] = 252; break;
                        case '\u00FD': result[i] = 253; break;
                        case '\u0163': result[i] = 254; break;
                        case '\u02D9': result[i] = 255; break;
                        default: result[i] = (byte)'?'; break;
                    }
                }
                return result;
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                return count;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }

            #endregion

            public override int CodePage
            {
                get { return 1250; }
            }

            public override string EncodingName
            {
                get { return "Central European (Windows)"; }
            }

            public override string WebName
            {
                get { return "windows-1250"; }
            }
        }

        // US-ASCII
        private sealed class Enc20127 : Encoding
        {
            #region conversion

            public override char[] GetChars(byte[] bytes, int index, int count)
            {
                var result = new char[count];
                for (var i = 0; i < result.Length; i++)
                {
                    var b = bytes[i + index];
                    result[i] = (b > 127) ? '?' : (char)b;
                }
                return result;
            }

            public override byte[] GetBytes(char[] chars, int index, int count)
            {
                var result = new byte[count];
                for (var i = 0; i < result.Length; i++)
                {
                    var c = chars[i + index];
                    result[i] = (c > 127) ? (byte)'?' : (byte)c;
                }
                return result;
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                return count;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }

            #endregion

            public override int CodePage
            {
                get { return 20127; }
            }

            public override string EncodingName
            {
                get { return "US-ASCII"; }
            }

            public override string WebName
            {
                get { return "us-ascii"; }
            }
        }
    }
}
