using System;

public static partial class Tools
{
    public static bool Equal(this string a, Enum b)
    {
        return a.Equals(b.ToString());
    }

    public static bool IsNullOrEmpty(this string v1)
    {
        return string.IsNullOrEmpty(v1);
    }
    
    public static bool IsNoNullOrNoEmpty(this string v1)
    {
        return !string.IsNullOrEmpty(v1);
    }
}