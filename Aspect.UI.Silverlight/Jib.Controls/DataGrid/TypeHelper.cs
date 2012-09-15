using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jib.Controls.DataGrid
{
    /// <summary>
    /// This class is bad....  I wish I knew a better way of determining if a type has the properties shown below
    /// </summary>
    public class TypeHelper
    {
        public static object ValueConvertor(Type type, string value)
        {

            if (type == typeof(byte) || type == typeof(byte?))
            {
                byte x;
                if (byte.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                sbyte x;
                if (sbyte.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                short x;
                if (short.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(ushort) || type == typeof(ushort?))
            {
                ushort x;
                if (ushort.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                int x;
                if (int.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(uint) || type == typeof(uint?))
            {
                uint x;
                if (uint.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                long x;
                if (long.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(ulong) || type == typeof(ulong?))
            {
                ulong x;
                if (ulong.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                float x;
                if (float.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                double x;
                if (double.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                decimal x;
                if (decimal.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                char x;
                if (char.TryParse(value, out x))
                    return x;
                else
                    return null;
            }
            return null;
        }

        public static bool IsValueType(Type type)
        {
            return type == typeof(byte) ||
                    type == typeof(sbyte) ||
                    type == typeof(short) ||
                    type == typeof(ushort) ||
                    type == typeof(int) ||
                    type == typeof(uint) ||
                    type == typeof(long) ||
                    type == typeof(ulong) ||
                    type == typeof(float) ||
                    type == typeof(double) ||
                    type == typeof(decimal) ||
                     type == typeof(bool) ||
                    type == typeof(char);
        }

        public static bool IsNullable(Type type)
        {
            return type != typeof(byte) &&
                type != typeof(sbyte) &&
                type != typeof(short) &&
                type != typeof(ushort) &&
                type != typeof(int) &&
                type != typeof(uint) &&
                type != typeof(long) &&
                type != typeof(ulong) &&
                type != typeof(float) &&
                type != typeof(double) &&
                type != typeof(decimal) &&
                type != typeof(char) &&
                type != typeof(bool) &&
                type != typeof(Single) &&
                type != typeof(DateTime);
        }

        public static bool IsNumbericType(Type p)
        {
            bool result = false;
            result = result || p == typeof(int);
            result = result || p == typeof(decimal);
            result = result || p == typeof(float);
            result = result || p == typeof(int?);
            result = result || p == typeof(decimal?);
            result = result || p == typeof(float?);
            return result;
        }

        public static bool IsStringType(Type p)
        {
            return !IsNumbericType(p);
        }



    }
}
