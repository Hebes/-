using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主菜单
/// </summary>
[NoDontDestroyOnLoad]
public class MainMenu : SM<MainMenu>
{
    public AudioClip menuMusic;
    public CanvasGroup mainPanel;
    public CanvasGroupController mainCG;
    [SerializeField] private GalleryMenu gallery;//画廊
    private Button _start;
    private Button _load;
    private Button _config;
    private Button _gallery;
    private Button _help;
    private Button _quit;
    
    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.I;

    private void Awake()
    {
        I = this;

        mainPanel = GetComponent<CanvasGroup>();
        _start = transform.Find("NavBar/Start").GetComponent<UnityEngine.UI.Button>();
        _load = transform.Find("NavBar/Load").GetComponent<UnityEngine.UI.Button>();
        _config = transform.Find("NavBar/Config").GetComponent<UnityEngine.UI.Button>();
        _gallery = transform.Find("NavBar/Gallery").GetComponent<UnityEngine.UI.Button>();
        _help = transform.Find("NavBar/Help").GetComponent<UnityEngine.UI.Button>();
        _quit = transform.Find("NavBar/Quit").GetComponent<UnityEngine.UI.Button>();
    }

    public void Start()
    {
        _start.onClick.AddListener(Click_StartNewGame);
        _load.onClick.AddListener(R.VNMenuSystem.OpenLoadPage);
        _config.onClick.AddListener(R.VNMenuSystem.OpenConfigPage);
        _gallery.onClick.AddListener(GalleryMenu.I.Open);
        _help.onClick.AddListener(R.VNMenuSystem.OpenHelpPage);
        _quit.onClick.AddListener(R.VNMenuSystem.Click_Quit);
        
        mainCG = new CanvasGroupController(this, mainPanel);
        menuMusic = R.Load<AudioClip>(ConfigMusic.Calm2);
        R.AudioSystem.StopAllSoundEffects();
        R.AudioSystem.StopAllTracks();
        R.AudioSystem.PlayTrack(menuMusic, channel: 0, startingVolume: 1);
    }

    public void Click_StartNewGame()
    {
        var yes = new UIConfirmationMenu.ConfirmationButton(R.LanguageSystem.Get("是"), StartNewGame);
        var no = new UIConfirmationMenu.ConfirmationButton(R.LanguageSystem.Get("否"), null);
        uiChoiceMenu.Show(R.LanguageSystem.Get("是否开始新游戏?"),yes,no);
    }

    public void LoadGame(VNGameSave file)
    {
        VNGameSave.activeFile = file;
        R.StartCoroutine(StartingGame());
    }

    private void StartNewGame()
    {
        VNGameSave.activeFile = new VNGameSave();
        R.StartCoroutine(StartingGame());
    }

    private IEnumerator StartingGame()
    {
        mainCG.Hide(speed: 0.3f);
        R.AudioSystem.StopTrack(0);
        while (mainCG.IsVisible)
            yield return null;
        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConfigScene.VisualNovel);
    }
}