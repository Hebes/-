using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视觉小说,数据库链接设置
/// </summary>
public class VNDatabaseLinkSetup : MonoBehaviour
{
    /// <summary>
    /// 设置外部链接
    /// </summary>
    public void SetupExternalLinks()
    {
        VariableStore.CreateVariable("VN.mainCharName", "",
            () => VNGameSave.activeFile.playerName,
            value => VNGameSave.activeFile.playerName = value);
    }
}