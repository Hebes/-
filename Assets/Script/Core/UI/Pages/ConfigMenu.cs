using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 配置菜单->场景1和2都有
/// </summary>
public class ConfigMenu : MenuPage
{
    public static ConfigMenu I { get; private set; }

    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    public UI_ITEMS ui;

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
        I = this;

        panels = new GameObject[2];
        panels[0] = transform.Find("Content/Panels/General").gameObject;
        panels[1] = transform.Find("Content/Panels/Audio").gameObject;

        anim = transform.Find("Content").GetComponent<UnityEngine.Animator>();

        var generalBtn = transform.Find("Content/PanelSelectionHeader/General").GetComponent<UnityEngine.UI.Button>();
        var audioBtn = transform.Find("Content/PanelSelectionHeader/Audio").GetComponent<UnityEngine.UI.Button>();
        generalBtn.onClick.AddListener(() => { OpenPanel("General"); });
        audioBtn.onClick.AddListener(() => { OpenPanel("Audio"); });
        ui.Awake(this);

        pageType = PageType.Config;
    }

    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();

        LoadConfig();
    }

    /// <summary>
    /// 加载游戏配置文件
    /// </summary>
    private void LoadConfig()
    {
        VN_Configuration.activeConfig = File.Exists(VN_Configuration.filePath)
            ? FileSystem.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.ENCRYPT)
            : new VN_Configuration();
        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"没有找到面板 '{panelName}' 在配置菜单中.");
            return;
        }

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    /// <summary>
    /// 设置可用分辨率
    /// </summary>
    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }

    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color button_selectedColor = new Color(1, 0.35f, 0, 1);
        private static Color button_unselectedColor = new Color(1f, 1f, 1f, 1);
        private static Color text_selectedColor = new Color(1, 1f, 0, 1);
        private static Color text_unselectedColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public static Color musicOnColor = new Color(1, 0.65f, 0, 1);
        public static Color musicOffColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Header("General")] public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoReaderSpeed;

        [Header("Audio")] public Slider musicVolume;
        public Image musicFill;
        public Slider sfxVolume;
        public Image sfxFill;
        public Slider voicesVolume;
        public Image voicesFill;
        public Sprite mutedSymbol; //静音状态标志
        public Sprite unmutedSymbol; //取消静音状态标志
        public Image musicMute;
        public Image sfxMute;
        public Image voicesMute;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.GetComponent<Image>().color = selectedA ? button_selectedColor : button_unselectedColor;
            B.GetComponent<Image>().color = !selectedA ? button_selectedColor : button_unselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_selectedColor : text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = !selectedA ? text_selectedColor : text_unselectedColor;
        }

        public void Awake(ConfigMenu configMenu)
        {
            Transform tr = configMenu.transform;
            fullscreen = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/Display/GameObject (1)/FullScreen").GetComponent<UnityEngine.UI.Button>();
            windowed = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/Display/GameObject (1)/Windows").GetComponent<UnityEngine.UI.Button>();
            resolutions = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/Resolution/GameObject/Dropdown").GetComponent<TMPro.TMP_Dropdown>();
            skippingContinue = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/SkipBehavior/GameObject (1)/ContinueSkipping").GetComponent<UnityEngine.UI.Button>();
            skippingStop = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/SkipBehavior/GameObject (1)/StopSkipping").GetComponent<UnityEngine.UI.Button>();
            architectSpeed = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/TextSpeed/GameObject (1)/ArchitectSpeed").GetComponent<UnityEngine.UI.Slider>();
            autoReaderSpeed = tr.Find("Content/Panels/General/Scroll View/Viewport/Content/LeftColumn/AutoReadSpeed/GameObject (2)/AutoReaderSpeed").GetComponent<UnityEngine.UI.Slider>();
            musicVolume = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/MusicVolume/GameObject/MusicVolume").GetComponent<UnityEngine.UI.Slider>();
            musicFill = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/MusicVolume/GameObject/MusicVolume/Fill Area/Fill").GetComponent<UnityEngine.UI.Image>();
            sfxVolume = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/SFXVolume/GameObject (1)/SFXVolume").GetComponent<UnityEngine.UI.Slider>();
            sfxFill = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/SFXVolume/GameObject (1)/SFXVolume/Fill Area/Fill").GetComponent<UnityEngine.UI.Image>();
            voicesVolume = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/VoiceVolume/GameObject (2)/VoiceVolume").GetComponent<UnityEngine.UI.Slider>();
            voicesFill = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/VoiceVolume/GameObject (2)/VoiceVolume/Fill Area/Fill").GetComponent<UnityEngine.UI.Image>();
            mutedSymbol = R.Load<Sprite>(ConfigImage.UIIconMusicOff);
            unmutedSymbol = R.Load<Sprite>(ConfigImage.UIIconMusicOn);
            musicMute = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/MusicVolume/musicMute").GetComponent<UnityEngine.UI.Image>();
            sfxMute = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/SFXVolume/sfxMute").GetComponent<UnityEngine.UI.Image>();
            voicesMute = tr.Find("Content/Panels/Audio/Scroll View (1)/Viewport/Content/LeftColumn/VoiceVolume/voicesMute").GetComponent<UnityEngine.UI.Image>();

            fullscreen.onClick.AddListener(() => { configMenu.SetDisplayToFullScreen(true); });
            windowed.onClick.AddListener(() => { configMenu.SetDisplayToFullScreen(false); });
            resolutions.onValueChanged.AddListener((t) => { configMenu.SetDisplayResolution(); });
            skippingContinue.onClick.AddListener(() => { configMenu.SetContinueSkippingAfterChoice(true); });
            skippingStop.onClick.AddListener(() => { configMenu.SetContinueSkippingAfterChoice(false); });
            architectSpeed.onValueChanged.AddListener((t) => { configMenu.SetTextArchitectSpeed(); });
            autoReaderSpeed.onValueChanged.AddListener((t) => { configMenu.SetAutoReaderSpeed(); });
            musicVolume.onValueChanged.AddListener((t) => { configMenu.SetMusicVolume(); });
            sfxVolume.onValueChanged.AddListener((t) => { configMenu.SetSFXVolume(); });
            voicesVolume.onValueChanged.AddListener((t) => { configMenu.SetVoicesVolume(); });
            musicMute.GetComponent<Button>().onClick.AddListener(configMenu.SetMusicMute);
            sfxMute.GetComponent<Button>().onClick.AddListener(configMenu.SetSFXMute);
            voicesMute.GetComponent<Button>().onClick.AddListener(configMenu.SetVoicesMute);
        }
    }

    //UI CALLABLE FUNCTIONS
    public void SetDisplayToFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.display_resolution = resolution;
        }
        else
            $"屏幕分辨率解析错误! [{resolution}]".LogError();
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;

        if (R.DialogueSystem != null)
            R.DialogueSystem.TextArchitect.speed = config.dialogueTextSpeed;
    }

    public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReadSpeed = ui.autoReaderSpeed.value;

        if (R.currentScene == ConfigScene.MainMenu || R.DialogueSystem == null) //TODO 需要测试
            return;

        AutoReader autoReader = R.DialogueSystem.autoReader;
        if (autoReader != null)
            autoReader.Speed = config.dialogueAutoReadSpeed;
    }

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value;
        R.AudioSystem.SetMusicVolume(config.musicVolume, config.musicMute);

        ui.musicFill.color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value;
        R.AudioSystem.SetSFXVolume(config.sfxVolume, config.sfxMute);

        ui.sfxFill.color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetVoicesVolume()
    {
        config.voicesVolume = ui.voicesVolume.value;
        R.AudioSystem.SetVoicesVolume(config.voicesVolume, config.voicesMute);

        ui.voicesFill.color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetMusicMute()
    {
        config.musicMute = !config.musicMute;
        ui.musicVolume.fillRect.GetComponent<Image>().color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol;

        R.AudioSystem.SetMusicVolume(config.musicVolume, config.musicMute);
    }

    public void SetSFXMute()
    {
        config.sfxMute = !config.sfxMute;
        ui.sfxVolume.fillRect.GetComponent<Image>().color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

        R.AudioSystem.SetSFXVolume(config.sfxVolume, config.sfxMute);
    }

    public void SetVoicesMute()
    {
        config.voicesMute = !config.voicesMute;
        ui.voicesVolume.fillRect.GetComponent<Image>().color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
        ui.voicesMute.sprite = config.voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;

        R.AudioSystem.SetVoicesVolume(config.voicesVolume, config.voicesMute);
    }
}