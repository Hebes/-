using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 文本构建-逐显
/// </summary>
public class TABuilder_Fade : TABuilder
{
    private int pretextlength = 0;//统计文字的长度，预文本长度

    public override Coroutine Build()
    {
        Prepare();

        return R.StartCoroutine(Building());
    }

    public override void ForceComplete()
    {
        architect.ShowText.ForceMeshUpdate();
    }
    //预备,隐藏所有的字
    private void Prepare()
    {
        architect.ShowText.text = architect.PreText;
        if (architect.PreText != "")
        {
            architect.ShowText.ForceMeshUpdate();
            pretextlength = architect.ShowText.textInfo.characterCount;
        }
        else
            pretextlength = 0;

        architect.ShowText.text += architect.TargetText;
        architect.ShowText.maxVisibleCharacters = int.MaxValue;
        architect.ShowText.ForceMeshUpdate();

        TMP_TextInfo textInfo = architect.ShowText.textInfo;

        Color colorVisible = new Color(architect.textColor.r, architect.textColor.g, architect.textColor.b, 1);
        Color colorHidden = new Color(architect.textColor.r, architect.textColor.g, architect.textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i = 0; i < textInfo.characterCount; i++)//计算文本中字符
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];//单个字符

            if (!charInfo.isVisible)//单个字是否显示
                continue;
            //如果单个字是显示的话
            if (i < pretextlength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }

        architect.ShowText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator Building()
    {
        int minRange = pretextlength;
        int maxRange = minRange + 1;

        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = architect.ShowText.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while(true)
        {
            float fadeSpeed = (architect.HurryUp ? architect.CharactersPerCycle * 5 : architect.CharactersPerCycle) * architect.speed * 4f;
            
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                //一个不可见的字符实际上可能指向文本中的第一个字符，从而导致一些奇怪的行为。
                if (!charInfo.isVisible || charInfo.index < minRange)
                    continue;

                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minRange++;
            }

            architect.ShowText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            
            if(lastCharacterIsInvisible || alphas[maxRange - 1] > alphaThreshold)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255)
                    break;
            }

            yield return null;
        }

        OnComplete();
    }
}