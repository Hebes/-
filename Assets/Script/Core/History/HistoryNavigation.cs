using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 历史上的导航
/// </summary>
public class HistoryNavigation : MonoBehaviour
{
    public int progress = 0;
    private HistoryState cachedState = null;
    private bool isOnCachedState = false;
    public bool isViewingHistory = false; //历史是否可见

    private TextMeshProUGUI statusText => R.UISystem.UIDialogue.historyState;
    public bool canNavigate => !R.DialogueSystem.ConversationManager.IsOnLogicalLine;
    private List<HistoryState> history => R.HistorySystem.history;

    public void GoForward()
    {
        if (!isViewingHistory || !canNavigate)
            return;

        HistoryState state = null;

        if (progress < history.Count - 1)
        {
            progress++;
            state = history[progress];
        }
        else
        {
            isOnCachedState = true;
            state = cachedState;
        }

        state.Load();

        if (isOnCachedState)
        {
            isViewingHistory = false;
            R.DialogueSystem.onUserPromptNext -= GoForward;
            statusText.text = "";
            R.DialogueSystem.OnStopViewingHistory();
        }
        else
            UpdateStatusText();
    }

    public void GoBack()
    {
        if (history.Count == 0 || (progress == 0 && isViewingHistory) || !canNavigate)
            return;

        progress = isViewingHistory ? progress - 1 : history.Count - 1;

        if (!isViewingHistory)
        {
            isViewingHistory = true;
            isOnCachedState = false;
            cachedState = HistoryState.Capture();

            R.DialogueSystem.onUserPromptNext += GoForward;
            R.DialogueSystem.OnStartViewingHistory();
        }

        HistoryState state = history[progress];
        state.Load();
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        statusText.text = $"{history.Count - progress}/{history.Count}";
    }
}