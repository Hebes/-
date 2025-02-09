using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 历史日志管理器
/// </summary>
public class HistoryLogManager : MonoBehaviour
{
    private const float LOG_STARTING_HEIGHT = 2f;
    private const float LOG_HEIGHT_PER_LINE = 2f;
    private const float LOG_DEFAULT_HEIGHT = 1f;
    private const float TEXT_DEFAULT_SCALE = 1f;

    private const string NAME_TEXT_NAME = "LogNameText";
    private const string DIALOGUE_TEXT_NAME = "LogDialogueText";

    private float logScaling = 1f;

    private List<HistoryLog> logs = new List<HistoryLog>();
    public bool isOpen { get; private set; } = false;
    private float textScaling => logScaling * 3f;

    private Animator anim;
    private GameObject logPrefab;
    private Slider logScaleSlider;

    private void Awake()
    {
        Transform root = R.UITotalRoot;
        anim = root.Find("Main/Root/CanvasOverrlay/HistoryLogs/Panel").GetComponent<UnityEngine.Animator>();
        logPrefab = root.Find("Main/Root/CanvasOverrlay/HistoryLogs/Panel/Scroll View/Viewport/Content/Log").GetComponent<UnityEngine.Transform>().gameObject;
        logScaleSlider = root.Find("Main/Root/CanvasOverrlay/HistoryLogs/Panel/Slider").GetComponent<UnityEngine.UI.Slider>();
    }

    private void OnEnable()
    {
        logScaleSlider.onValueChanged.AddListener(SetLogScaling);
    }

    private void OnDisable()
    {
        logScaleSlider.onValueChanged.RemoveListener(SetLogScaling);
    }


    public void Open()
    {
        if (isOpen)
            return;

        anim.Play("LogOpen");

        isOpen = true;
    }

    public void Close()
    {
        if (!isOpen)
            return;

        anim.Play("LogClose");

        isOpen = false;
    }

    public void AddLog(HistoryState state)
    {
        if (logs.Count >= HistorySystem.HISTORY_CACHE_LIMIT)
        {
            DestroyImmediate(logs[0].container);
            logs.RemoveAt(0);
        }

        CreateLog(state);
    }

    private void CreateLog(HistoryState state)
    {
        HistoryLog log = new HistoryLog
        {
            container = logPrefab.Instantiate(logPrefab.transform.parent)
        };

        log.container.SetActive(true);

        log.nameText = log.container.FindComponentByName<TextMeshProUGUI>(NAME_TEXT_NAME);
        log.dialogueText = log.container.FindComponentByName<TextMeshProUGUI>(DIALOGUE_TEXT_NAME);

        if (state.dialogue.currentSpeaker == string.Empty)
        {
            log.nameText.text = string.Empty;
        }
        else
        {
            log.nameText.text = state.dialogue.currentSpeaker;
            log.nameText.font = HistoryCache.LoadFont(state.dialogue.speakerFont);
            log.nameText.color = state.dialogue.speakerNameColor;
            log.nameFontSize = TEXT_DEFAULT_SCALE * state.dialogue.speakerScale;
            log.nameText.fontSize = log.nameFontSize + textScaling;
        }

        log.dialogueText.text = state.dialogue.currentDialogue;
        log.dialogueText.font = HistoryCache.LoadFont(state.dialogue.dialogueFont);
        log.dialogueText.color = state.dialogue.dialogueColor;
        log.dialogueFontSize = TEXT_DEFAULT_SCALE * state.dialogue.dialogueScale;
        log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

        FitLogToText(log);

        logs.Add(log);
    }

    /// <summary>
    /// 将日志转换为文本,这里处理文本的高度
    /// </summary>
    /// <param name="log"></param>
    private void FitLogToText(HistoryLog log)
    {
        RectTransform rect = (RectTransform)log.dialogueText.transform;
        ContentSizeFitter textCSF = log.dialogueText.GetComponent<ContentSizeFitter>();

        textCSF.SetLayoutVertical();

        LayoutElement logLayout = log.container.GetComponent<LayoutElement>();
        float height = rect.rect.height;

        float perc = height / LOG_DEFAULT_HEIGHT;
        float extraScale = (LOG_HEIGHT_PER_LINE * perc) - LOG_HEIGHT_PER_LINE;
        float scale = LOG_STARTING_HEIGHT + extraScale;

        logLayout.preferredHeight = scale + textScaling;

        logLayout.preferredHeight += 2f * logScaling;
    }

    /// <summary>
    /// 设置日志缩放
    /// </summary>
    public void SetLogScaling(float arg0)
    {
        logScaling = logScaleSlider.value;

        foreach (HistoryLog log in logs)
        {
            log.nameText.fontSize = log.nameFontSize + textScaling;
            log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

            FitLogToText(log);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < logs.Count; i++)
            DestroyImmediate(logs[i].container);

        logs.Clear();
    }

    public void Rebuild()
    {
        foreach (var state in R.HistorySystem.history)
            CreateLog(state);
    }
}