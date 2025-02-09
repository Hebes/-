using UnityEngine;

/// <summary>
/// 文本构建-立刻显示
/// </summary>
public class TABuilder_Instant : TABuilder
{
    public override Coroutine Build()
    {
        architect.ShowText.color = architect.ShowText.color;
        architect.ShowText.text = architect.fullTargetText;
        architect.ShowText.ForceMeshUpdate();
        architect.ShowText.maxVisibleCharacters = architect.ShowText.textInfo.characterCount;

        OnComplete();

        return null;
    }
}