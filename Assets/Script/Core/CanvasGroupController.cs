using System.Collections;
using UnityEngine;

/// <summary>
/// 画布组控制
/// </summary>
public class CanvasGroupController
{
    private const float DEFAULT_FADE_SPEED = 3f;

    private MonoBehaviour owner;
    public CanvasGroup rootCG;

    private Coroutine co_showing = null;
    private Coroutine co_hiding = null;
    private bool IsShowing => co_showing != null;
    private bool IsHiding => co_hiding != null;

    public bool IsFading => IsShowing || IsHiding;
    public bool IsVisible => co_showing != null || rootCG.alpha > 0;

    public float alpha
    {
        get => rootCG.alpha;
        set => rootCG.alpha = value;
    }

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCg)
    {
        this.owner = owner;
        this.rootCG = rootCg;
    }

    public Coroutine Show(float speed = 1f, bool immediate = false)
    {
        if (co_showing.Has()) return co_showing;

        if (co_hiding.Has())
        {
            R.StopCoroutine(co_hiding);
            co_hiding = null;
        }

        return co_showing = R.StartCoroutine(Fading(1, speed, immediate));
    }

    public Coroutine Hide(float speed = 1f, bool immediate = false)
    {
        if (co_hiding.Has()) return co_hiding;

        if (co_showing.Has())
        {
            R.StopCoroutine(co_showing);
            co_showing = null;
        }

        return co_hiding = R.StartCoroutine(Fading(0, speed, immediate));
    }

    private IEnumerator Fading(int alpha, float speed, bool immediate)
    {
        CanvasGroup cg = rootCG;

        if (immediate)
            cg.alpha = alpha;

        while (cg.alpha != alpha)
        {
            yield return null;
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED * speed);
        }

        co_showing = null;
        co_hiding = null;
    }

    public void SetInteractableState(bool active)
    {
        rootCG.interactable = active;
        rootCG.blocksRaycasts = active;
    }
}