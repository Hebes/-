using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏启动器
/// </summary>
public class GameLunch : BaseBehaviour
{
    private readonly List<IBehaviour> _systemList = new List<IBehaviour>();

    private void Awake()
    {
        _systemList.Add(R.UISystem);
        _systemList.Add(R.DialogueSystem);

        //自定义生命周期,组件获取有冲突的都从这边按照自定义顺序进行
        foreach (IBehaviour mono in _systemList)
            mono.OnGetComponent();
        foreach (IBehaviour mono in _systemList)
            mono.OnNewClass();
        foreach (IBehaviour mono in _systemList)
            mono.OnListener();
        foreach (IBehaviour mono in _systemList)
            mono.OnInitialize();
    }
}

public interface IBehaviour
{
    //按照下面顺序初始化
    public virtual void OnGetComponent()
    {
    }

    public virtual void OnNewClass()
    {
    }

    public virtual void OnListener()
    {
    }

    public virtual void OnInitialize()
    {
    }
}