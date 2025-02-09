using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 异常退出
/// </summary>
public class AbnormalExitSystem : SM<AbnormalExitSystem>
{
   private void OnEnable()
   {
      Application.wantsToQuit += OnBeforeQuit;
   }
   
   private void OnDisable()
   {
      Application.wantsToQuit -= OnBeforeQuit;
   }
   
   private bool OnBeforeQuit()
   {
      // 在游戏即将退出时执行清理操作
      Debug.Log("游戏即将退出。执行清理操作.");
      string filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/LogOut/游戏闪退.txt";
      string content = "游戏闪退.";
      File.WriteAllText(filePath, content);
      // 在这里可以添加需要执行的清理操作，比如保存游戏数据等
      return true; // 返回true允许游戏退出，返回false则阻止游戏退出
   }
}