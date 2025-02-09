using System.Collections;
using UnityEngine;

/// <summary>
/// 打字机
/// </summary>
public class TABuilder_Typewriter : TABuilder
{
    public override Coroutine Build()
    {
        Prepare();
        return architect.ShowText.StartCoroutine(Building());
    }

    public override void ForceComplete()
    {
        architect.ShowText.ForceMeshUpdate();
        architect.ShowText.maxVisibleCharacters = architect.ShowText.textInfo.characterCount;
    }

    private void Prepare()
    {
        architect.ShowText.color = architect.ShowText.color;
        architect.ShowText.maxVisibleCharacters = 0;
        architect.ShowText.text = architect.PreText;

        if (architect.PreText != "")
        {
            architect.ShowText.ForceMeshUpdate();
            architect.ShowText.maxVisibleCharacters = architect.ShowText.textInfo.characterCount;
        }

        architect.ShowText.text += architect.TargetText;
        architect.ShowText.ForceMeshUpdate();
    }

    private IEnumerator Building()
    {
        while (architect.ShowText.maxVisibleCharacters < architect.ShowText.textInfo.characterCount)
        {
            architect.ShowText.maxVisibleCharacters += architect.HurryUp ? architect.CharactersPerCycle * 2 : architect.CharactersPerCycle;

            yield return new WaitForSeconds(0.015f / (architect.HurryUp ? architect.speed * 5 : architect.speed));
        }

        OnComplete();
    }
}