using UnityEngine;

/// <summary>
/// 携程
/// </summary>
public static partial class Tools
{
    public static bool IsNull(this Coroutine coroutine) => coroutine == null;
    public static bool Has(this Coroutine coroutine) => coroutine != null;
}