/// <summary>
/// 配置字符串
/// </summary>
public class ConfigString
{
    #region Regex 语法手册 https: //developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions/Cheatsheet

    /*
     * {c}    =clear
     * {a}    =append
     * {wc n} =wait clear number
     * {wa n} =wait append number
     */
    public const string SegmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

    public const string CommandRegexPattern = @"[\w\[\]]*[^\s]\(";

    public static readonly string SpeakerPattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";

    public const string NAMECAST_ID = " as ";
    public const string POSITIONCAST_ID = " at ";
    public const string EXPRESSIONCAST_ID = " [";
    public const char AXISDELIMITER_ID = ':';
    public const char EXPRESSIONLAYER_JOINER = ',';
    public const char EXPRESSIONLAYER_DELIMITER = ':';
    
    //角色预制体路径
    public const string CHARACTER_NAME_ID = "<charname>";
    public static readonly string CharacterRootPath = $"Character/{CHARACTER_NAME_ID}";
    public static readonly string CharacterPrefabPath = $"{CharacterRootPath}/Character - [{CHARACTER_NAME_ID}]";

    #endregion
}