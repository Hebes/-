using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual Novel视觉小说 系统
/// </summary>
[RequireComponent(typeof(VNDatabaseLinkSetup))]
public class VNSystem : SM<VNSystem>
{
    private VisualNovelSO config => DB.I.visualNovelSo;
    public Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        VNDatabaseLinkSetup linkSetup = GetComponent<VNDatabaseLinkSetup>();
        linkSetup.SetupExternalLinks();
        VNGameSave.activeFile ??= new VNGameSave();
        mainCamera = R.MainCamera;
    }

    private void Start()
    {
        LoadGame();
    }

    private void LoadGame()
    {
        if (VNGameSave.activeFile.newGame)
        {
            List<string> lines = FileSystem.ReadTextAsset(config.startingFile);
            Conversation start = new Conversation(lines);
            R.DialogueSystem.Say(start);
        }
        else
        {
            VNGameSave.activeFile.Activate();
        }
    }
}