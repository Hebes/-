/// <summary>
/// 配置字符串
/// </summary>
public class ConfigString
{
    #region Regex 语法手册 https: //developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions/Cheatsheet

   

    


    
   
    
   



    //角色预制体路径
    public const string CHARACTER_CASTING_ID = " as ";
    public const string CHARACTER_NAME_ID = "<charname>";
    public static readonly string CharacterRootPathFormat = $"Character/{CHARACTER_NAME_ID}";
    public static readonly string CharacterPrefabNameFormat = $"Character - [{CHARACTER_NAME_ID}]";
    public static readonly string CharacterPrefabPathFormat = $"{CharacterRootPathFormat}/{CharacterPrefabNameFormat}";

    #endregion
}