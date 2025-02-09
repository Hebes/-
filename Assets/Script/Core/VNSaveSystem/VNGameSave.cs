using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 视觉小说游戏存档
/// </summary>
[System.Serializable]
public class VNGameSave
{
    public static VNGameSave activeFile = null;

    public const bool ENCRYPT = false;
    public const string FILE_TYPE = ".vns";
    public const string SCREENSHOT_FILE_TYPE = ".jpg";
    public const float SCREENSHOT_DOWNSCALE_AMOUNT = 0.25f;

    public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
    public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";


    public string playerName;
    public int slotNumber = 1;

    public bool newGame = true;
    public string[] activeConversations;
    public HistoryState activeState;
    public HistoryState[] historyLogs;
    public VN_VariableData[] variables;

    public string timestamp;

    public static VNGameSave Load(string filePath, bool activateOnLoad = false)
    {
        VNGameSave save = FileSystem.Load<VNGameSave>(filePath, ENCRYPT);

        activeFile = save;

        if (activateOnLoad)
            save.Activate();

        return save;
    }

    public void Save()
    {
        newGame = false;

        activeState = HistoryState.Capture();
        historyLogs = R.HistorySystem.history.ToArray();
        activeConversations = GetConversationData();
        variables = GetVariableData();

        timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

        $"存档路径：{screenshotPath}".Log();
        ScreenshotMasterSystem.CaptureScreenshot(R.MainCamera, Screen.width, Screen.height, SCREENSHOT_DOWNSCALE_AMOUNT, screenshotPath);
        string saveJson = JsonUtility.ToJson(this);
        FileSystem.Save(filePath, saveJson, ENCRYPT);
        $"截图存档路径：{screenshotPath}   游戏文件存档路径：{filePath}".Log();
    }

    public void Activate()
    {
        activeState?.Load();

        R.HistorySystem.history = historyLogs.ToList();
        R.HistorySystem.logManager.Clear();
        R.HistorySystem.logManager.Rebuild();

        SetVariableData();

        SetConversationData();

        R.DialogueSystem.Prompt.Hide();
    }

    private string[] GetConversationData()
    {
        List<string> retData = new List<string>();
        var conversations = R.DialogueSystem.ConversationManager.GetConversationQueue();

        for (int i = 0; i < conversations.Length; i++)
        {
            var conversation = conversations[i];
            string data = "";

            if (conversation.file != string.Empty)
            {
                var compressedData = new VN_ConversationDataCompressed();
                compressedData.fileName = conversation.file;
                compressedData.progress = conversation.GetProgress();
                compressedData.startIndex = conversation.fileStartIndex;
                compressedData.endIndex = conversation.fileEndIndex;
                data = JsonUtility.ToJson(compressedData);
            }
            else
            {
                var fullData = new VN_ConversationData();
                fullData.conversation = conversation.GetLines();
                fullData.progress = conversation.GetProgress();
                data = JsonUtility.ToJson(fullData);
            }

            retData.Add(data);
        }

        return retData.ToArray();
    }

    private void SetConversationData()
    {
        for (int i = 0; i < activeConversations.Length; i++)
        {
            try
            {
                string data = activeConversations[i];
                Conversation conversation = null;

                var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                {
                    conversation = new Conversation(fullData.conversation, fullData.progress);
                }
                else
                {
                    var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                    if (compressedData != null && compressedData.fileName != string.Empty)
                    {
                        TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);

                        int count = compressedData.endIndex - compressedData.startIndex;

                        List<string> lines = FileSystem.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                        conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                    }
                    else
                    {
                        Debug.LogError($"未知的对话格式！无法使用数据从VNGameSave重新加载对话 '{data}'");
                    }
                }

                if (conversation != null && conversation.GetLines().Count > 0)
                {
                    if (i == 0)
                        R.DialogueSystem.ConversationManager.StartConversation(conversation);
                    else
                        R.DialogueSystem.ConversationManager.Enqueue(conversation);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"在提取保存的会话数据时遇到错误! {e}");
                continue;
            }
        }
    }

    private VN_VariableData[] GetVariableData()
    {
        List<VN_VariableData> retData = new List<VN_VariableData>();

        foreach (var database in VariableStore.databases.Values)
        {
            foreach (var variable in database.variables)
            {
                VN_VariableData variableData = new VN_VariableData();
                variableData.name = $"{database.name}.{variable.Key}";
                string val = $"{variable.Value.Get()}";
                variableData.value = val;
                variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString();
                retData.Add(variableData);
            }
        }

        return retData.ToArray();
    }

    private void SetVariableData()
    {
        foreach (var variable in variables)
        {
            string val = variable.value;

            switch (variable.type)
            {
                case "System.Boolean":
                    if (bool.TryParse(val, out bool b_val))
                    {
                        VariableStore.TrySetValue(variable.name, b_val);
                        continue;
                    }

                    break;
                case "System.Int32":
                    if (int.TryParse(val, out int i_val))
                    {
                        VariableStore.TrySetValue(variable.name, i_val);
                        continue;
                    }

                    break;
                case "System.Single":
                    if (float.TryParse(val, out float f_val))
                    {
                        VariableStore.TrySetValue(variable.name, f_val);
                        continue;
                    }

                    break;
                case "System.String":
                    VariableStore.TrySetValue(variable.name, val);
                    continue;
            }

            $"无法解释变量类型. {variable.name} = {variable.type}".LogError();
        }
    }
}