using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Character_Sprite : Character
{
    private const string SPRITE_RENDERER_PARENT_NAME = "Renderers";
    private const string SPRITESHEET_DEFAULT_SHEETNAME = "Default";
    private const char SPRITESHEET_TEX_SPRITE_DELIMITTER =',';// '-';

    public readonly List<CharacterSpriteLayer> Layers = new List<CharacterSpriteLayer>();
    private string _artAssetsDirectory;
    private CanvasGroup rootCg => Root.gameObject.FindComponent<CanvasGroup>();

    public override bool isVisible
    {
        get => co_revealing.Has() || Mathf.Approximately(rootCg.alpha, 1);
        set => rootCg.alpha = value ? 1 : 0;
    }

    public Character_Sprite(string name, CharacterConfigData config, GameObject prefab, string rootCharacterFolder) : base(name, config, prefab)
    {
        if (Root == null)
        {
            $"正在执行{name},请检查创建的角色".LogError();
        }

        rootCg.alpha = ENABLE_ON_START ? 1 : 0;
        _artAssetsDirectory = rootCharacterFolder + "/Images";

        GetLayers();
    }

    private void GetLayers()
    {
        Transform rendererRoot = Animator.transform.Find(SPRITE_RENDERER_PARENT_NAME);
        if (rendererRoot == null) return;
        for (int i = 0; i < rendererRoot.transform.childCount; i++)
        {
            Transform child = rendererRoot.transform.GetChild(i);
            Image rendererImage = child.GetComponentInChildren<Image>();
            if (rendererImage != null)
            {
                CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                Layers.Add(layer);
                child.name = $"Layer:{i}";
            }
        }
    }

    public void SetSprite(Sprite sprite, int layer = 0)
    {
        Layers[layer].SetSprite(sprite);
    }

    public Sprite GetSprite(string spriteName)
    {
        //尝试从角色的精灵字典中加载精灵
        if (Config.spriteList.Count > 0)
        {
            foreach (SpriteData spriteData in Config.spriteList)
            {
                if (spriteName.Equals(spriteData.name))
                    return spriteData.sprite;
            }
        }

        switch (Config.characterType)
        {
            case CharacterType.Text:
            case CharacterType.Live2D:
            case CharacterType.Model3D:
            case CharacterType.Sprite:
                return R.Load<Sprite>($"{_artAssetsDirectory}/{spriteName}");
            case CharacterType.SpriteSheet:
                string[] data = spriteName.Split(SPRITESHEET_TEX_SPRITE_DELIMITTER);
                Sprite[] spriteArray = Array.Empty<Sprite>(); 
                
                // if (data.Length == 2)
                // {
                //     string textureName = data[0];
                //     spriteName = data[1];
                //     
                //     path = $"{_artAssetsDirectory}/{textureName}";
                // }
                // else
                // {
                //     string   path = $"{_artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}";
                // }

                string  path = $"{_artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}";

                Sprite[] defSprite = R.LoadAll<Sprite>(path);
                if (defSprite.Length == 0)
                    throw new Exception($"角色名称错误");
                if (data.Length == 2)
                {
                    spriteName = data[1];
                }
                Sprite tempValue = Array.Find(defSprite, sprite => sprite.name == spriteName);
                if (tempValue == null)
                    throw new Exception($"角色表情错误");
                return tempValue;
            default: throw new Exception("错误类型这个是Sprite脚本");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="layer">0层级身体,1层级脸部</param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1)
    {
        CharacterSpriteLayer spriteLayer = Layers[layer];
        return spriteLayer.TransitionSprite(sprite, speed);
    }

    public override IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
    {
        float targetAlpha = show ? 1f : 0;

        while (!Mathf.Approximately(rootCg.alpha, targetAlpha))
        {
            rootCg.alpha = Mathf.MoveTowards(rootCg.alpha, targetAlpha, 3f * R.DeltaTime * speedMultiplier);
            yield return null;
        }

        co_revealing = null;
        co_hiding = null;
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);
        color = displayColor;
        foreach (CharacterSpriteLayer layer in Layers)
        {
            layer.StopChangingColor();
            layer.SetColor(color);
        }
    }

    protected override IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
    {
        Color targetColor = displayColor;
        foreach (CharacterSpriteLayer layer in Layers)
        {
            if (immediate)
            {
                layer.SetColor(targetColor);
            }
            else
            {
                layer.TransitionColor(targetColor, speedMultiplier);
            }
        }

        yield return null;
        while (Layers.Any(l => l.co_changingColor.Has()))
            yield return null;
        co_highlighting = null;
    }

    protected override IEnumerator ChangingColor(Color color, float speed)
    {
        foreach (CharacterSpriteLayer layer in Layers)
            layer.TransitionColor(color, speed);
        yield return null;
        while (Layers.Any(l => l.co_changingColor.Has()))
            yield return null;
        co_changingColor = null;
    }

    protected override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
    {
        foreach (CharacterSpriteLayer layer in Layers)
        {
            if (faceLeft)
                layer.FaceLeft(speedMultiplier, immediate);
            else
                layer.FaceRight(speedMultiplier, immediate);
        }

        yield return null;
        while (Layers.Any(l => l.co_flipping.Has()))
            yield return null;
        co_flipping = null;
    }

    public override void OnReceiveCastingExpression(int layer, string expression)
    {
        base.OnReceiveCastingExpression(layer, expression);
        Sprite sprite = GetSprite(expression);
        if (sprite == null)
            throw new Exception($"没有找到{expression}");
        TransitionSprite(sprite, layer);
    }
}