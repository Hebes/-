using System;
using UnityEngine.UI;

[NoDontDestroyOnLoad]
public class UIMenus : BaseBehaviour
{
    private Button Return;
    private Button Config;
    private Button Save;
    private Button Load;
    private Button Home;
    private Button Help;
    private Button Quit;

    private void Awake()
    {
        Return = transform.Find("RootMenus/NavigationBar/Return").GetComponent<UnityEngine.UI.Button>();
        Config = transform.Find("RootMenus/NavigationBar/Config").GetComponent<UnityEngine.UI.Button>();
        Save = transform.Find("RootMenus/NavigationBar/Save").GetComponent<UnityEngine.UI.Button>();
        Load = transform.Find("RootMenus/NavigationBar/Load").GetComponent<UnityEngine.UI.Button>();
        Home = transform.Find("RootMenus/NavigationBar/Home").GetComponent<UnityEngine.UI.Button>();
        Help = transform.Find("RootMenus/NavigationBar/Help").GetComponent<UnityEngine.UI.Button>();
        Quit = transform.Find("RootMenus/NavigationBar/Quit").GetComponent<UnityEngine.UI.Button>();
    }

    private void Start()
    {
        Return.onClick.AddListener(R.VNMenuSystem.CloseRoot);
        Config.onClick.AddListener(R.VNMenuSystem.OpenConfigPage);
        Save.onClick.AddListener(R.VNMenuSystem.OpenSavePage);
        Load.onClick.AddListener(R.VNMenuSystem.OpenLoadPage);
        Home.onClick.AddListener(R.VNMenuSystem.Click_Home);
        Help.onClick.AddListener(R.VNMenuSystem.OpenHelpPage);
        Quit.onClick.AddListener(R.VNMenuSystem.Click_Quit);
    }
}