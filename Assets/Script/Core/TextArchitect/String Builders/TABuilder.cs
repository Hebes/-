using UnityEngine;

/// <summary>
/// Stellar Studio的文本架构系统：tabbuilder是textarchitarchitect使用的所有字符串构建器的基础。
/// </summary>
public abstract class TABuilder
{
    public TextArchitect architect = null;

    public delegate void TA_Event();
    public event TA_Event onComplete;

    /// <summary>
    /// 这是每个构建器类型前面的字符串/文本。每个tabbuilder都有一个名字，前面有这个前缀，用来命名类。Instant = PREFIX + Instant。淡出=前缀+淡出
    /// </summary>
    public const string CLASS_NAME_PREFIX = "TABuilder_";

    /// <summary>
    /// 使用不同的方法来呈现文本。
    /// </summary>
    public enum BuilderTypes
    {
        Instant,
        Typewriter,
        Fade
    }

    public virtual Coroutine Build() => null;

    public virtual void ForceComplete()
    {

    }

    protected void OnComplete() => onComplete?.Invoke();
}