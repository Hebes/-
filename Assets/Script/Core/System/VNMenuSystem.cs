using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// 视觉小说菜单系统
/// </summary>
[NoDontDestroyOnLoad]
public class VNMenuSystem :  SM<VNMenuSystem>
{
    //这个要设置不同的物体
    public CanvasGroup root;
    public MenuPage[] pages;

    private MenuPage activePage = null;
    private bool isOpen = false;
    private CanvasGroupController rootCG;

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.I;

    private void Awake()
    {
        I = this;
        //切换场景会自动重新Awake 然后赋值
        root = GameObject.Find("RootMenus").GetComponent<CanvasGroup>();
        pages = new MenuPage[3];
        pages[0] = root.transform.FindComponentByName<SaveAndLoadMenu>("SaveAndLoadMenu");
        pages[1] = root.transform.FindComponentByName<ConfigMenu>("Config");
        pages[2] = root.transform.FindComponentByName<HelpPage>("Help");
        rootCG = new CanvasGroupController(this, root);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            rootCG.Hide();
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            rootCG.Show();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            rootCG.alpha = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            rootCG.alpha = 1;
    }


    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.Save;
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.Load;
        OpenPage(page);
    }

    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    private void OpenPage(MenuPage page)
    {
        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    public void OpenRoot()
    {
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        rootCG.SetInteractableState(false);
        isOpen = false;
    }

    public void Click_Home()
    {
        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(ConfigScene.MainMenu);
    }

    public void Click_Quit()
    {
        var yes = new UIConfirmationMenu.ConfirmationButton("是", Application.Quit);
        var no = new UIConfirmationMenu.ConfirmationButton("不", null);
        uiChoiceMenu.Show("退出桌面?", yes, no);
    }
}