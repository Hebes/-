using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HistoryLogManager))]
[RequireComponent(typeof(HistoryNavigation))]
public class HistorySystem : MonoBehaviour
{
    public const int HISTORY_CACHE_LIMIT = 100;
    public static HistorySystem instance { get; private set; }
    public List<HistoryState> history = new List<HistoryState>();


    private HistoryNavigation navigation;
    public bool isViewingHistory => navigation.isViewingHistory;

    public HistoryLogManager logManager { get; private set; }

    private void Awake()
    {
        instance = this;
        navigation = GetComponent<HistoryNavigation>();
        logManager = GetComponent<HistoryLogManager>();
    }

    void Start()
    {
        R.DialogueSystem.onClear += LogCurrentState;
    }

    public void LogCurrentState()
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