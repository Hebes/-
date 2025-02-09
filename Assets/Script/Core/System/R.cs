using System.Collections;
using UnityEngine;

/// <summary>
/// 全局管理
/// </summary>
public static class R
{
    //System
    public static DialogueSystem DialogueSystem => DialogueSystem.I; // 对话系统
    public static LanguageSysatem LanguageSystem => LanguageSysatem.I; // 多语言系统
    public static PlayerInputSystem PlayerInputSystem => PlayerInputSystem.I; // 玩家输入管理器
    public static CommandSystem CommandSystem => CommandSystem.I; // 指令系统
    public static CharacterSystem CharacterSystem => CharacterSystem.I; // 角色管理器
    public static GraphicPanelSystem GraphicPanelSystem => GraphicPanelSystem.I; // 图形面板
    public static AudioSystem AudioSystem => AudioSystem.I; // 音效系统
    public static TagSystem TagSystem => TagSystem.I; // 标签系统
    public static UISystem UISystem => UISystem.I; // UI系统
    public static InputPanelSystem InputPanelSystem => InputPanelSystem.I; // 玩家输入系统
    public static LogicalLineSystem LogicalLineSystem => LogicalLineSystem.I; // 逻辑线路系统
    public static ChoicePanelSystem ChoicePanelSystem => ChoicePanelSystem.I; // 选择面板系统
    public static HistorySystem HistorySystem => HistorySystem.I; // 历史系统
    public static VNSystem VNSystem => VNSystem.I; // 视觉小说系统
    public static AbnormalExitSystem AbnormalExitSystem => AbnormalExitSystem.I; // 异常退出系统
    public static VNMenuSystem VNMenuSystem => VNMenuSystem.I; // 视觉小说菜单系统
    public static CheckSystem CheckSystem => CheckSystem.I; // 全局检查系统

    //DB
    public static DB DB => DB.I;//数据库
    public static CharacterConfigSo CharacterConfigSo => DB.I.DSCSO.CharacterConfigurationAssetr;
    
    //UIComponent
    public static Transform UITotalRoot => GameObject.Find("RenderGroups").transform;

    //Camera
    public static Camera MainCamera => VNSystem.mainCamera;

    //Coroutine
    public static Coroutine StartCoroutine(IEnumerator routine) => LanguageSystem.StartCoroutine(routine);
    public static void StopCoroutine(Coroutine routine) => LanguageSystem.StopCoroutine(routine);

    //time
    public static float DeltaTime => Time.deltaTime;

    //Load
    public static T Load<T>(string path) where T : Object => Resources.Load<T>(path);
    public static Object Load(string path) => Resources.Load(path, typeof(Object));
    public static T[] LoadAll<T>(string path) where T : Object => Resources.LoadAll<T>(path);

    //Scene
    public static string currentScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

    //Instantiate
    public static T Instantiate<T>(this T original, Transform parent) where T : Object => Object.Instantiate<T>(original, parent, false);
}