using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public string name;
    public RectTransform root = null;

    protected Character(string name)
    {
        this.name = name;
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