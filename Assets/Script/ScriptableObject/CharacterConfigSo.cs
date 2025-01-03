using UnityEngine;

/// <summary>
/// 角色配置OS
/// </summary>
[CreateAssetMenu(fileName = "角色配置", menuName = "对话系统/角色配置", order = 0)]
public class CharacterConfigSo : ScriptableObject
{
    public CharacterConfigData[] characterArray;

    public CharacterConfigData GetConfig(string characterName)
    {
        characterName = characterName.ToLower();
        for (int i = 0; i < characterArray.Length; i++)
        {
            CharacterConfigData data = characterArray[i];
            if(characterName.Equals(data.name.ToLower()) || characterName.Equals(data.alias.ToLower()))
                return data.Copy();
        }

        return CharacterConfigData.DefaultData();
    } 
    
}