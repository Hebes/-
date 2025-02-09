using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家输入管理器
/// 这个脚本必须在场景中添加
/// </summary>
[NoDontDestroyOnLoad]
public class PlayerInputSystem : SM<PlayerInputSystem>
{
    private PlayerInput input;
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

    private void Awake()
    {
        input = gameObject.GetComponent<PlayerInput>();
        input.actions = DB.I.inputActionAsset;
        input.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        
        actions.Add((input.actions["Next"], OnNext));
        actions.Add((input.actions["HistoryBack"], OnHistoryBack));
        actions.Add((input.actions["HistoryForward"], OnHistoryForward));
        actions.Add((input.actions["HistoryLogs"], OnHistoryToggleLog));
    }

    private void OnEnable()
    {
        foreach ((InputAction action, Action<InputAction.CallbackContext> command) inputAction in actions)
            inputAction.action.performed += inputAction.command;
    }

    private void OnDisable()
    {
        foreach ((InputAction action, Action<InputAction.CallbackContext> command) inputAction in actions)
            inputAction.action.performed -= inputAction.command;
    }

    private void OnHistoryToggleLog(InputAction.CallbackContext obj)
    {
        HistoryLogManager logs = R.HistorySystem.logManager;

        if (logs.isOpen)
            logs.Close();
        else
            logs.Open();
    }

    private void OnHistoryForward(InputAction.CallbackContext obj)
    {
        R.HistorySystem.GoForward();
    }

    private void OnHistoryBack(InputAction.CallbackContext obj)
    {
        R.HistorySystem.GoBack();
    }

    private void OnNext(InputAction.CallbackContext obj)
    {
        R.DialogueSystem.OnUserPrompt_Next();
    }
}