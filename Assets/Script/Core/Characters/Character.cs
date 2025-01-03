using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class Character
{
    protected Character(string name, CharacterConfigData config, GameObject prefab)
    {
        DisplayName = Name = name;
        Config = config;
        if (prefab)
        {
            GameObject go = Object.Instantiate(prefab,(RectTransform)R.UICharacters.transform);
            go.SetActive(true);
            Root = go.transform as RectTransform;
            Animator = go.FindComponent<Animator>();
        }
    }

    public string Name;
    public string DisplayName;
    public RectTransform Root = null;
    public Animator Animator;

    public CharacterConfigData Config = null;

    //Coroutines
    protected Coroutine co_revealing;
    protected Coroutine co_hiding;
    public bool isRevealing => co_revealing != null;
    public bool isHiding => co_hiding != null;
    public virtual bool isVisible => false; //是否可见

    public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

    public Coroutine Say(List<string> dialogue)
    {
        R.DialogueSystem.ShowSpeakerName(DisplayName);
        UpdateTextCustomizationsOnScreen();
        return R.DialogueSystem.Say(dialogue);
    }

    public void SetNameFont(TMP_FontAsset font) => Config.nameFont = font;
    public void SetDialogueFont(TMP_FontAsset font) => Config.dialogueFont = font;
    public void SetNameColor(Color color) => Config.nameColor = color;
    public void SetDialogueColor(Color color) => Config.dialogueColor = color;
    public void ResetConfigurationData() => Config = R.CharacterSystem.GetCharacterConfig(Name);
    public void UpdateTextCustomizationsOnScreen() => R.DialogueSystem.ApplySpeakerDataToDialogueContainer(Config);


    public virtual Coroutine Show()
    {
        if (isRevealing)
            return co_revealing;

        if (isHiding)
            R.CharacterSystem.StopCoroutine(co_hiding);

        co_revealing = R.CharacterSystem.StartCoroutine(ShowingOrHiding(true));
        return co_revealing;
    }

    public virtual Coroutine Hide()
    {
        if (isHiding)
            return co_hiding;
        if (isRevealing)
            R.CharacterSystem.StopCoroutine(co_revealing);

        co_hiding = R.CharacterSystem.StartCoroutine(ShowingOrHiding(false));
        return co_hiding;
    }

    public virtual IEnumerator ShowingOrHiding(bool show)
    {
        yield return null;
    }

    public enum CharacterType
    {
        Text,
        Sprite,
        SpriteSheet,
        Live2D,
        ModeL3D
    }
}