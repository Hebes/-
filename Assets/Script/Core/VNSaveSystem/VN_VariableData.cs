using System.Collections.Generic;

/// <summary>
/// 视觉小说变量数据
/// </summary>
[System.Serializable]
public class VN_VariableData
{
    public string name;
    public string value;
    public string type;
}

/// <summary>
/// 视觉小说会话数据压缩
/// </summary>
[System.Serializable]
public class VN_ConversationDataCompressed
{
    public string fileName;
    public int startIndex, endIndex;
    public int progress;
}

/// <summary>
/// 视觉小说会话数据
/// </summary>
[System.Serializable]
public class VN_ConversationData
{
    public List<string> conversation = new List<string>();
    public int progress;
}