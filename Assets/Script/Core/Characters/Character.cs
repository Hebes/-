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
            //这里才Instantiate
            GameObject go = Object.Instantiate(prefab, (RectTransform)R.UISystem.UICharacters.transform);
            go.name = R.CharacterSystem.FormatCharacterPath(ConfigString.CharacterPrefabNameFormat, name);
            go.SetActive(true);
            Root = go.transform as RectTransform;
            Animator = go.FindComponent<Animator>();
        }
    }

    public enum CharacterType
    {
        Text,
        Sprite,
        SpriteSheet,
        Live2D,
        Model3D
    }

    public CharacterConfigData Config;

    public string Name= "";
    public string displayName = "";
    public string castingName = "";
    public string DisplayName;
    public RectTransform Root;
    public Animator Animator;
    public Vector2 targetPosition { get; private set; }
    
    //Coroutines
    public Coroutine co_revealing;
    public Coroutine co_hiding;
    protected Coroutine co_moving;
    protected Coroutine co_changingColor;
    protected Coroutine co_highlighting;
    protected Coroutine co_flipping;

    private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;
    public const bool ENABLE_ON_START = false;
    public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = true;
    public const string ANIMATION_REFRESH_TRIGGER = "ReFresh";
    public int priority { get; protected set; }

    public bool highlighted { get; protected set; } = true;
    public bool isHighlighting => highlighted && co_highlighting.Has();
    public bool isUnHighlighting => !highlighted && co_highlighting.Has();
    protected bool facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
    public bool isFacingLeft => facingLeft;
    public bool isFacingRight => !facingLeft;
    public Color color { get; protected set; } = Color.white;
    protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
    protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
    protected Color highlightedColor => color;

    public virtual bool isVisible { get; set; } //是否可见

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
    public void ResetConfigurationData() => Config = R.CharacterSystem.GetCharacterConfig(Name, getOriginal: true);
    public void UpdateTextCustomizationsOnScreen() => R.DialogueSystem.ApplySpeakerDataToDialogueContainer(Config);


    public virtual Coroutine Show(float speedMultiplier = 1f)
    {
        if (co_revealing.Has())
            R.StopCoroutine(co_revealing);
        if (co_hiding.Has())
            R.StopCoroutine(co_hiding);
        co_revealing = R.StartCoroutine(ShowingOrHiding(true, speedMultiplier));
        return co_revealing;
    }

    public virtual Coroutine Hide(float speedMultiplier = 1f)
    {
        if (co_hiding.Has())
            R.StopCoroutine(co_hiding);
        if (co_revealing.Has())
            R.StopCoroutine(co_revealing);
        co_hiding = R.StartCoroutine(ShowingOrHiding(false, speedMultiplier));
        return co_hiding;
    }

    public virtual void SetColor(Color color)
    {
        this.color = color;
    }

    public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
    {
        yield return null;
    }

    #region 设置位置

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="position"></param>
    public virtual void SetPosition(Vector2 position)
    {
        if (Root == null) return;
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
        Root.anchorMin = minAnchorTarget;
        Root.anchorMax = maxAnchorTarget;
        
        targetPosition = position;
    }

    protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
    {
        Vector2 padding = Root.anchorMax - Root.anchorMin;
        float maxY = 1f - padding.y;
        float maxX = 1f - padding.x;
        Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
        Vector2 maxAnchorTarget = minAnchorTarget + padding;
        return (minAnchorTarget, maxAnchorTarget);
    }

    #endregion

    #region 移动位置

    /// <summary>
    /// 移动位置
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="speed"></param>
    /// <param name="smooth"></param>
    /// <returns></returns>
    public virtual Coroutine MoveToPosition(Vector2 positon, float speed = 2f, bool smooth = false)
    {
        if (Root == null)
            return null;
        if (co_moving.Has())
            R.StopCoroutine(co_moving);
        co_moving = R.StartCoroutine(MovingToPosition(positon, speed, smooth));
        targetPosition = positon;
        return co_moving;
    }

    private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
    {
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
        Vector2 padding = Root.anchorMax - Root.anchorMin;
        while (Root.anchorMin != minAnchorTarget || Root.anchorMax != maxAnchorTarget)
        {
            if (smooth)
                Root.anchorMin = Vector2.Lerp(Root.anchorMin, minAnchorTarget, speed * R.DeltaTime);
            else
                Root.anchorMin = Vector2.MoveTowards(Root.anchorMin, minAnchorTarget, speed * R.DeltaTime * 0.35f);
            Root.anchorMax = Root.anchorMin + padding;
            if (smooth && Vector2.Distance(Root.anchorMin, minAnchorTarget) <= 0.001f)
            {
                Root.anchorMin = minAnchorTarget;
                Root.anchorMax = maxAnchorTarget;
                break;
            }

            yield return null;
        }

        co_moving = null;
    }

    #endregion

    #region 过度颜色

    /// <summary>
    /// 过度颜色
    /// </summary>
    /// <param name="color"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public Coroutine TransitionColor(Color color, float speed = 1f)
    {
        this.color = color;
        if (co_changingColor.Has())
            R.StopCoroutine(co_changingColor);
        co_changingColor = R.StartCoroutine(ChangingColor(displayColor, speed));
        return co_changingColor;
    }

    protected virtual IEnumerator ChangingColor(Color color1, float speed)
    {
        "颜色变化不适用于这个字符类型！".Log();
        yield return null;
    }

    #endregion

    #region 高亮

    public Coroutine Highlight(float speed = 1f, bool immediate = false)
    {
        if (isHighlighting ||isUnHighlighting)
             R.StopCoroutine(co_highlighting);
        highlighted = true;
        co_highlighting = R.StartCoroutine(Highlighting(speed, immediate));
        return co_highlighting;
    }

    public Coroutine UnHighlight(float speed = 1f, bool immediate = false)
    {
        if (isHighlighting ||isUnHighlighting)
            R.StopCoroutine(co_highlighting);
        highlighted = false;
        co_highlighting = R.StartCoroutine(Highlighting(speed, immediate));
        return co_highlighting;
    }


    protected virtual IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
    {
        yield return null;
    }

    #endregion

    #region 转向

    public Coroutine Flip(float speed = 1, bool immediate = false)
    {
        if (isFacingLeft)
            return FaceRight(speed, immediate);
        return FaceLeft(speed, immediate);
    }

    public Coroutine FaceLeft(float speed = 1, bool immediate = false)
    {
        if (co_flipping.Has()) R.StopCoroutine(co_flipping);
        facingLeft = true;
        co_flipping = R.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
        return co_flipping;
    }

    public Coroutine FaceRight(float speed = 1, bool immediate = false)
    {
        if (co_flipping.Has()) R.StopCoroutine(co_flipping);
        facingLeft = facingLeft;
        co_flipping = R.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
        return co_flipping;
    }

    protected virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
    {
        yield return null;
    }

    #endregion

    #region 设置优先级

    public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
    {
        this.priority = priority;
        if (autoSortCharactersOnUI)
            R.CharacterSystem.SortCharacters();
    }

    public virtual void OnReceiveCastingExpression(int layer, string expression)
    {
        return;
    }

    #endregion

    #region 角色动画

    public void PlayAnim(string animName)
    {
        Animator.Play(animName);
    }

    public void Animate(string animation)
    {
        Animator.SetTrigger(animation);
    }

    public void Animate(string animation, bool state)
    {
        Animator.SetBool(animation, state);
        Animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
    }

    #endregion
}