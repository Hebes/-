using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 历史系统
/// </summary>
[NoDontDestroyOnLoad]
[RequireComponent(typeof(HistoryLogManager))]
[RequireComponent(typeof(HistoryNavigation))]
public class HistorySystem : SM<HistorySystem>
{
    public const int HISTORY_CACHE_LIMIT = 100;
    public List<HistoryState> history = new List<HistoryState>();
    private HistoryNavigation navigation;

    public HistoryLogManager logManager { get; private set; }
    public bool isViewingHistory => navigation.isViewingHistory;

    private void Awake()
    {
        navigation = GetComponent<HistoryNavigation>();
        logManager = GetComponent<HistoryLogManager>();
    }
    private void Start()
    {
        R.DialogueSystem.onClear += LogCurrentState;
    }

    private void LogCurrentState()
    {
        HistoryState state = HistoryState.Capture();
        history.Add(state);
        logManager.AddLog(state);

        if (history.Count > HISTORY_CACHE_LIMIT)
            history.RemoveAt(0);
    }
    public void LoadState(HistoryState state)
    {
        state.Load();
    }

    public void GoForward() => navigation.GoForward();
    public void GoBack() => navigation.GoBack();
}