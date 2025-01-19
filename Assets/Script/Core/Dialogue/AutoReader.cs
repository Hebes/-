using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 自动阅读
/// </summary>
public class AutoReader : BaseBehaviour
{
    private const int DEFAULT_CHARACTERS_READ_PER_SECOND = 18;
    private const float READ_TIME_PADDING = 0.5f;
    private const float MAX_READ_TIME = 99f; //最大读取时间
    private const float MIN_READ_TIME = 1f; //最小读取时间
    private const string STATUS_TEXT_AUTO = "Auto"; //状态文本自动
    private const string STATUS_TEXT_SKIP = "Skipping"; //状态文本跳过

    public bool Skip { get; set; } = false;

    public float Speed { get; set; } = 1f;

    public bool IsOn => co_running != null;
    public Coroutine co_running;
    private TextMeshProUGUI StatusText => R.UISystem.UIDialogue.autoReadStatus;
    [HideInInspector] public bool allowToggle = true;

    public void Initialize()
    {
        StatusText.text = string.Empty;
    }

    public void Enable()
    {
        if (co_running.Has())
            return;

        co_running = StartCoroutine(AutoRead());
    }


    public void Disable()
    {
        if (co_running.IsNull()) return;
        StopCoroutine(co_running);
        Skip = false;
        co_running = null;
        StatusText.text = string.Empty;
    }

    private IEnumerator AutoRead()
    {
        TextArchitect architect = R.DialogueSystem.TextArchitect;
        ConversationManager conversationManager = R.DialogueSystem.ConversationManager;

        //如果没有对话要监视，什么也不做。
        if (!conversationManager.isRunning)
        {
            Disable();
            yield break;
        }

        if (!architect.isBuilding && architect.currentText != string.Empty)
            R.DialogueSystem.OnSystemPrompt_Next();

        while (conversationManager.isRunning)
        {
            //阅读等待
            if (!Skip)
            {
                while (!architect.isBuilding && !conversationManager.IsWaitingOnAutoTimer)
                    yield return null;

                yield return new WaitForSeconds(0.02f);

                float timeStarted = Time.time;

                while (architect.isBuilding || conversationManager.IsWaitingOnAutoTimer)
                    yield return null;

                float timeToRead = Mathf.Clamp(((float)architect.tmpro.textInfo.characterCount / DEFAULT_CHARACTERS_READ_PER_SECOND), MIN_READ_TIME, MAX_READ_TIME);
                timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                timeToRead = (timeToRead / Speed) + READ_TIME_PADDING;

                $"等待 [{timeToRead}秒] 在 '{architect.currentText}'".Log();

                yield return new WaitForSeconds(timeToRead);
            }
            else //Skip
            {
                architect.ForceComplete();
                yield return new WaitForSeconds(0.05f);
            }
 
            R.DialogueSystem.OnSystemPrompt_Next();
        }

        Disable();
    }

    public void Toggle_Auto()
    {
        if (!allowToggle)
            return;

        bool prevState = Skip;
        Skip = false;

        if (prevState)
        {
            Enable();
        }
        else
        {
            if (co_running.Has())
                Disable();
            else
                Enable();
        }

        if (co_running.Has())
            StatusText.text = STATUS_TEXT_AUTO;
    }

    public void Toggle_Skip()
    {
        if (!allowToggle)
            return;

        bool prevState = Skip;
        Skip = true;

        if (!prevState)
        {
            Enable();
        }
        else
        {
            if (co_running.Has())
                Disable();
            else
                Enable();
        }

        if (co_running.Has())
            StatusText.text = STATUS_TEXT_SKIP;
    }
}