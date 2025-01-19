using System.Collections;
using UnityEngine;

/// <summary>
/// 全局管理
/// </summary>
public class R
{
    public static DialogueSystem DialogueSystem => DialogueSystem.I; // 对话系统
    public static LanguageSysatem LanguageSystem => LanguageSysatem.I; // 多语言系统
    public static PlayerInputSystem PlayerInputSystem => PlayerInputSystem.I; // 玩家输入管理器
    public static CommandSystem CommandSystem => CommandSystem.I; // 指令系统
    public static CharacterSystem CharacterSystem => CharacterSystem.I; // 角色管理器
    public static GraphicPanelSystem GraphicPanelSystem => GraphicPanelSystem.I; // 图形面板
    public static AudioSystem AudioSystem => AudioSystem.I; // 音效系统
    public static TagSystem TagSystem => TagSystem.I; // 标签系统
    public static UISystem UISystem => UISystem.I; // UI系统
    public static AssetLoadSystem AssetLoadSystem => AssetLoadSystem.I; // 资源加载系统
    public static InputPanelSystem InputPanelSystem => InputPanelSystem.I; // 玩家输入系统
    public static LogicalLineSystem LogicalLineSystem => LogicalLineSystem.I; // 逻辑线路系统
    public static ChoicePanelSystem ChoicePanelSystem => ChoicePanelSystem.I; // 选择面板系统


    public static Coroutine StartCoroutine(IEnumerator routine) => DialogueSystem.StartCoroutine(routine);
    public static void StopCoroutine(Coroutine routine) => DialogueSystem.StopCoroutine(routine);
    public static T Load<T>(string path) where T : Object => AssetLoadSystem.I.Load<T>(path);
    public static float DeltaTime => Time.deltaTime;
}