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

    public void Show(string nameValue)
    {
        root.SetActive(true);
        if (!nameValue.IsNullOrEmpty())
            NameText.text = nameValue;
    }
    
    public void Hide()
    {
        root.SetActive(false);
    }
}