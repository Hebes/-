/// <summary>
/// 指令数据拓展
/// </summary>
public abstract class CMD_DatabaseExtension
{
    public static void Extend(CommandDataBase database)
    {
    }

    /// <summary>
    /// 将数据转换为参数
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startingIndex"></param>
    /// <returns></returns>
    public static CommandParameters ConvertDataToParameters(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
}