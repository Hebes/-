﻿using UnityEngine;

/// <summary>
/// 菜单页面
/// </summary>
public class MenuPage : BaseBehaviour
{
    public enum PageType
    {
        SaveAndLoad,
        Config,
        Help
    }

    public PageType pageType;

    private const string OPEN = "Open";
    private const string CLOSE = "Close";
    public Animator anim;

    public virtual void Open()
    {
        anim.SetTrigger(OPEN);
    }

    public virtual void Close(bool closeAllMenus = false)
    {
        anim.SetTrigger(CLOSE);

        if (closeAllMenus)
            R.VNMenuSystem.CloseRoot();
    }
}