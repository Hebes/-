using UnityEngine;

/// <summary>
/// 角色配置OS
/// </summary>
[CreateAssetMenu(fileName = "角色配置", menuName = "对话系统/角色配置", order = 0)]
public class CharacterConfigSo : ScriptableObject
{
    public CharacterConfigData[] characterArray;

    /// <summary>
    /// 获取配置文件
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public CharacterConfigData GetConfig(string characterName, bool safe = true)
    {
        characterName = characterName.ToLower().Trim();
        for (int i = 0; i < characterArray.Length; i++)
        {
            CharacterConfigData data = characterArray[i];
            if(characterName.Equals(data.name.ToLower()) || characterName.Equals(data.alias.ToLower()))
                return safe ? data.Copy() : data;
        }

        return CharacterConfigData.DefaultData();
    } 
    
}