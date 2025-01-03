public class R
{
    /// <summary>
    /// 对话系统
    /// </summary>
    public static DialogueSystem DialogueSystem => DialogueSystem.I;

    /// <summary>
    /// 多语言系统
    /// </summary>
    public static LanguageSysatem LanguageSystem => LanguageSysatem.I;

    /// <summary>
    /// 名字控制器
    /// </summary>
    public static NameContainer NameContainer => NameContainer.I;

    /// <summary>
    /// 对话内容控制器
    /// </summary>
    public static DialogueContainer DialogueContainer => DialogueContainer.I;
    
    /// <summary>
    /// 玩家输入管理器
    /// </summary>
    public static PlayerInputManager PlayerInputManager => PlayerInputManager.I;
    
    /// <summary>
    /// 指令系统
    /// </summary>
    public static CommandSystem CommandSystem => CommandSystem.I;
}