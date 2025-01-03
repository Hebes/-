using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 对话=>发言者数据
/// </summary>
public class DL_SPEAKER_DATA
{
    public string name;
    public string castName;

    /// <summary>
    /// 这个名称将显示在对话框中，以显示正在说话的人
    /// </summary>
    public string displayName => !castName.IsNullOrEmpty() ? castName : name;

    public Vector2 castPosition;

    public readonly List<(int layer, string expression)> CastExpressions = new List<(int layer, string expression)>();

    /// <summary>
    /// 演讲者数据投射
    /// </summary>
    /// <param name="rawSpeaker"></param>
    public DL_SPEAKER_DATA(string rawSpeaker)
    {
        MatchCollection matches = Regex.Matches(rawSpeaker, ConfigString.SpeakerPattern);
        //填充此数据以避免对值的空引用
        castName = String.Empty;
        castPosition = Vector2.zero;
        if (matches.Count == 0)
        {
            name = rawSpeaker;
        }
        else
        {
            //如果没有匹配，那么这一整行就是说话人的名字
            int index = matches[0].Index;
            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0;
                int endIndex = 0;

                if (match.Value == ConfigString.NAMECAST_ID)
                {
                    startIndex = match.Index + ConfigString.NAMECAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == ConfigString.POSITIONCAST_ID)
                {
                    startIndex = match.Index + ConfigString.POSITIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                    string[] axis = castPos.Split(ConfigString.AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);
                    float.TryParse(axis[0], out castPosition.x);
                    if (axis.Length > 1)
                    {
                        float.TryParse(axis[1], out castPosition.y);
                    }
                }
                else if (match.Value == ConfigString.EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + ConfigString.EXPRESSIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));
                    CastExpressions = castExp.Split(ConfigString.EXPRESSIONLAYER_JOINER).Select(x =>
                    {
                        var parts = x.Trim().Split(ConfigString.EXPRESSIONLAYER_DELIMITER);
                        return (int.Parse(parts[0]), parts[1]);
                    }).ToList();
                }
            }
        }
    }
}