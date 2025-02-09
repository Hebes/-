using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// UI系统
/// </summary>
public class UISystem : SM<UISystem>
{
    public UIDialogue UIDialogue => UIDialogue.I;
    public UICharacters UICharacters => UICharacters.I;
    public UIPlayerInput UIPlayerInput => UIPlayerInput.I;
    public UIChoicePanel UIChoicePanel => UIChoicePanel.I;
                                 
}
