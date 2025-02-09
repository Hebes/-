using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 精灵层
/// 表情等
/// </summary>
public class CharacterSpriteLayer
{
    public CharacterSpriteLayer(Image renderer, int layer = 0)
    {
        this.layer = layer;
        this.renderer = renderer;
    }

    public int layer { get; private set; } = 0;
    public Image renderer { get; private set; } = null;
    public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();

    private readonly List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

    private const float DEFAULT_TRANSITION_SPEED = 3f;
    private float transitionSpeedMultiplier = 1;


    public Coroutine co_changingColor;
    private Coroutine co_transitioningLayer;
    private Coroutine co_levelingAlpha;
    public Coroutine co_flipping;

    private bool isFacingLeft = Character.DEFAULT_ORIENTATION_IS_FACING_LEFT;

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }

    public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
    {
        if (sprite == renderer.sprite) return null;
        if (co_transitioningLayer.Has())
            R.StopCoroutine(co_transitioningLayer);
        co_transitioningLayer = R.StartCoroutine(TransitioningSprite(sprite, speed));
        return co_transitioningLayer;
    }


    private Image CreateRenderer(Transform parent)
    {
        Image newRenderer = renderer.Instantiate(parent);
        oldRenderers.Add(rendererCG);
        newRenderer.name = renderer.name;
        renderer = newRenderer;
        renderer.gameObject.SetActive(true);
        rendererCG.alpha = 0;
        return newRenderer;
    }

    private Coroutine TryStartLevelingAlphas()
    {
        if (co_levelingAlpha.Has())
            R.StopCoroutine(co_levelingAlpha);
        co_levelingAlpha = R.StartCoroutine(RunAlphaLeveLing());
        return co_levelingAlpha;
    }

    public Coroutine FaceLeft(float speed = 1, bool immediate = false)
    {
        if (co_flipping.Has()) R.StopCoroutine(co_flipping);
        isFacingLeft = true;
        co_flipping = R.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));
        return co_flipping;
    }

    public Coroutine FaceRight(float speed = 1, bool immediate = false)
    {
        if (co_flipping.Has()) R.StopCoroutine(co_flipping);
        isFacingLeft = false;
        co_flipping = R.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));
        return co_flipping;
    }

    public void SetColor(Color color)
    {
        renderer.color = color;
        foreach (CanvasGroup oldCg in oldRenderers)
        {
            oldCg.GetComponent<Image>().color = color;
        }
    }

    public Coroutine TransitionColor(Color color, float speed)
    {
        if (co_changingColor.Has())
            R.StopCoroutine(co_changingColor);
        co_changingColor = R.StartCoroutine(ChangingColor(color, speed));
        return co_changingColor;
    }

    public void StopChangingColor()
    {
        if (co_changingColor.IsNull()) return;
        R.StopCoroutine(co_changingColor);
        co_changingColor = null;
    }

    private IEnumerator ChangingColor(Color color, float speed)
    {
        Color oldColor = renderer.color;
        List<Image> oldImages = new List<Image>();
        foreach (CanvasGroup oldCg in oldRenderers)
            oldImages.Add(oldCg.GetComponent<Image>());
        float colorPercent = 0;
        while (colorPercent < 1)
        {
            colorPercent += DEFAULT_TRANSITION_SPEED * speed * R.DeltaTime;
            renderer.color = Color.Lerp(oldColor, color, colorPercent);
            for (int i = oldImages.Count - 1; i >= 0; i--)
            {
                Image image = oldImages[i];
                if (image != null)
                    image.color = renderer.color;
                else
                    oldImages.RemoveAt(i);
            }

            yield return null;
        }

        co_changingColor = null;
    }

    private IEnumerator RunAlphaLeveLing()
    {
        while (rendererCG.alpha < 1 || oldRenderers.Any(oldCg => oldCg.alpha > 0))
        {
            float speed = DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplier * R.DeltaTime;
            rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1, speed);
            for (int i = oldRenderers.Count - 1; i >= 0; i--)
            {
                CanvasGroup oldCg = oldRenderers[i];
                oldCg.alpha = Mathf.MoveTowards(oldCg.alpha, 0, speed);
                if (oldCg.alpha <= 0)
                {
                    oldRenderers.RemoveAt(i);
                    Object.Destroy(oldCg.gameObject);
                }
            }

            yield return null;
        }

        co_levelingAlpha = null;
    }

    private IEnumerator TransitioningSprite(Sprite sprite, float speed)
    {
        transitionSpeedMultiplier = speed;
        Image newRenderer = CreateRenderer(renderer.transform.parent);
        newRenderer.sprite = sprite;
        yield return TryStartLevelingAlphas();
        co_transitioningLayer = null;
    }

    private IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
    {
        float xScale = faceLeft ? 1 : -1;
        Vector3 newScale = new Vector3(xScale, 1, 1);
        if (!immediate)
        {
            Image newRenderer = CreateRenderer(renderer.transform.parent);
            newRenderer.transform.localScale = newScale;
            transitionSpeedMultiplier = speedMultiplier;
            TryStartLevelingAlphas();
            while (co_levelingAlpha.Has())
                yield return null;
        }
        else
        {
            renderer.transform.localScale = newScale;
        }

        co_flipping = null;
    }
}