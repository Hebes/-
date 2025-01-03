using System;

public static partial class Tools
{
    /// <summary>
    /// 是否是枚举值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static bool IsInEnum<TEnum>(this string value, bool ignoreCase = false) where TEnum : struct, IConvertible
    {
        if (!ignoreCase)
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        string[] names = Enum.GetNames(typeof(TEnum));
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(value, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }


    /// <summary>
    /// 尝试转换枚举
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <param name="result">新的枚举</param>
    /// <typeparam name="TEnum">枚举</typeparam>
    /// <returns></returns>
    public static bool TryParse<TEnum>(string value, out TEnum result, bool ignoreCase = false) where TEnum : struct
    {
        if (Enum.IsDefined(typeof(TEnum), value))
        {
            result = (TEnum)((object)Enum.Parse(typeof(TEnum), value, true));
            return true;
        }

        if (ignoreCase)
        {
            string[] names = Enum.GetNames(typeof(TEnum));
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = (TEnum)((object)Enum.Parse(typeof(TEnum), names[i]));
                    return true;
                }
            }
        }

        result = default(TEnum);
        return false;
    }

    /// <summary>
    /// 转枚举
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <typeparam name="TEnum">枚举</typeparam>
    /// <returns></returns>
    public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase = false) where TEnum : struct, IConvertible
    {
        TryParse<TEnum>(value, out TEnum result, ignoreCase);
        return result;
    }

    public static string[] ToArray<TEnum>(this System.Object obj) where TEnum : struct, IConvertible
    {
        return ToArray(typeof(TEnum));
    }

    public static string[] ToArray(this Type type)
    {
        return Enum.GetNames(type);
    }
}