using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI系统
/// </summary>
public class UISystem : SM<UISystem>, IBehaviour
{
    private readonly List<IUIBehaviour> _uiBehaviourList = new List<IUIBehaviour>();

    public UIDialogue UIDialogue => UIDialogue.I;
    public UIPlayerInteraction UIPlayerInteraction => UIPlayerInteraction.I;
    public UICharacters UICharacters => UICharacters.I;
    public UIPlayerInput UIPlayerInput => UIPlayerInput.I;
    public UIChoicePanel UIChoicePanel => UIChoicePanel.I;

    public void OnGetComponent()
    {
        _uiBehaviourList.Add(UIDialogue);
        _uiBehaviourList.Add(UIPlayerInteraction);
        _uiBehaviourList.Add(UIPlayerInput);
        _uiBehaviourList.Add(UIChoicePanel);

        foreach (var uiBehaviour in _uiBehaviourList)
            uiBehaviour.OnGetComponent();
    }

    public void OnNewClass()
    {
        foreach (var uiBehaviour in _uiBehaviourList)
            uiBehaviour.OnNewClass();
    }

    public void OnListener()
    {
        foreach (var uiBehaviour in _uiBehaviourList)
            uiBehaviour.OnListener();
    }
    
    public  void OnInitialize()
    {
        foreach (var uiBehaviour in _uiBehaviourList)
            uiBehaviour.OnInitialize();
    }
}

public interface IUIBehaviour
{
    //按照下面顺序初始化
    public void OnGetComponent();

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