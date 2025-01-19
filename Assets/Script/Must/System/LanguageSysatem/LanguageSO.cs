using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="多语言",menuName ="对话系统/多语言", order =0)]
public class LanguageSO : ScriptableObject
{
   public List<LanguageData> LanguageList = new List<LanguageData>();
}

