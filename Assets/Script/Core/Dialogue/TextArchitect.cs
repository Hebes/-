using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 文本构建(添加内容等)
/// {c}         =clear
/// {a}         =append
/// {wc n}      =wait clear number
/// {wa n}      =wait append number
/// </summary>
public class TextArchitect
{
    private float speedMultiplier = 1;
    private const float baseSpeed = 1;
    private int characterMultiplier = 1;
    public bool hurryUp = false; // 是否加快

    public string targetText = "";
    public string preText = "";
    private int preTextLength = 0;

    private TextMeshProUGUI tmpro_ui => R.UISystem.UIDialogue.dialogueText;
    private TextMeshPro tmpro_world;
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;
    public string currentText => tmpro.text;
    public string fullTargetText => preText + targetText;

    public Color textColor
    {
        get => tmpro.color;
        set => tmpro.color = value;
    }

    public float speed
    {
        get => baseSpeed * speedMultiplier;
        set => speedMultiplier = value;
    }

    public enum BuildMethod
    {
        /// <summary>
        /// 立刻显示
        /// </summary>
        Instant,

        /// <summary>
        /// 打字机
        /// </summary>
        TypeWriter,

        /// <summary>
        /// 逐渐显示
        /// </summary>
        Fade,
    }

    public BuildMethod buildMethod = R.DialogueSystem.Config.buildMethod; //BuildMethod.TypeWriter;//

    public int CharactersPerCycle => speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3;


    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;
        Stop();
        return buildProcess = tmpro.StartCoroutine(Building());
    }

    /// <summary>
    /// 向文本架构中已经存在的内容追加文本。
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;
        Stop();
        return buildProcess = tmpro.StartCoroutine(Building());
    }

    public Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public void Stop()
    {
        if (!isBuilding)
        {
            return;
        }

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();
        switch (buildMethod)
        {
            case BuildMethod.Instant:
                break;
            case BuildMethod.TypeWriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.Fade:
                yield return Build_Fade();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        OnComplete();
    }

    /// <summary>
    /// 动画结束后
    /// </summary>
    private void OnComplete()
    {
        buildProcess = null;
    }

    /// <summary>
    /// 强制完成
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.Instant:
                break;
            case BuildMethod.TypeWriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.Fade:
                tmpro.ForceMeshUpdate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Stop();
        OnComplete();
    }
    
    /// <summary>
    /// 立即将文本应用到对象
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        tmpro.text = targetText;
        //builder.ForceComplete();
    }

    /// <summary>
    /// 逐渐消失
    /// </summary>
    /// <returns></returns>
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;
        int maxRange = minRange + 1;
        byte alphaThreshold = 15;
        TMP_TextInfo textInfo = tmpro.textInfo;
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];
        while (true)
        {
            float fadeSpeed = (hurryUp ? CharactersPerCycle * 5 : CharactersPerCycle) * speed * 4f;
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];
                if (alphas[i] >= 255)
                    minRange++;
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || lastCharacterIsInvisible)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 构建打字机
    /// </summary>
    /// <returns></returns>
    private IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? CharactersPerCycle * 5 : CharactersPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.Instant:
                Prepare_Instant();
                break;
            case BuildMethod.TypeWriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.Fade:
                Prepare_Fade();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;
        if (!string.IsNullOrEmpty(preText))
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if (!string.IsNullOrEmpty(preText))
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
        {
            preTextLength = 0;
        }

        preTextLength = 0;
        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color colorVisable = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            if (i < preTextLength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisable;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                {
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
                }
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}