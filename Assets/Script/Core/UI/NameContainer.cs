using System;
using TMPro;
using UnityEngine;

public class NameContainer : SingletonMono<NameContainer>
{
    public GameObject root;
    public TextMeshProUGUI NameText;

    private void Awake()
    {
        NameText = transform.Find("Image/NameText").GetComponent<TMPro.TextMeshProUGUI>();
        root = NameText.gameObject;
    }
    
    public void SetNameColor(Color color) => NameText.color = color;
    public void SetNameFont(TMP_FontAsset font) => NameText.font = font;

    public void Show(string nameValue)
    {
        root.SetActive(true);
        if (!nameValue.IsNullOrEmpty())
            NameText.text = R.LanguageSystem.GetLanguage(nameValue) ;
    }
    
    public void Hide()
    {
        root.SetActive(false);
    }
}