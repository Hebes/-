using System;

/// <summary>
/// 视觉小说
/// </summary>
public class CMD_DatabaseExtension_VisualNovel : CMD_DatabaseExtension
{
    public new static void Extend(CommandDataBase database)
    {
        //变量赋值
        database.AddCommand("SetPlayerName", new Action<string>(SetPlayerNameVariable));
    }

    private static void SetPlayerNameVariable(string data)
    {
        VNGameSave.activeFile.playerName = data;
    }
}