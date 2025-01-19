using System.Collections.Generic;
using UnityEngine;
using static CharacterData.AnimationData;


[System.Serializable]
public class CharacterData
{
    public string characterName;
    public string castingName;
    public string displayName;
    public bool enabled;
    public Color color;
    public int priority;
    public bool isHighlighted;
    public bool isFacingLeft;
    public Vector2 position;
    public CharacterConfigCache characterConfig;

    public string animationJSON;
    public string dataJSON;

    [System.Serializable]
    public class CharacterConfigCache
    {
        public string name;
        public string alias;

        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public string nameFont;
        public string dialogueFont;

        public float nameFontScale = 1f;
        public float dialogueFontScale = 1f;

        public CharacterConfigCache(CharacterConfigData reference)
        {
            name = reference.name;
            alias = reference.alias;
            characterType = reference.characterType;

            nameColor = reference.nameColor;
            dialogueColor = reference.dialogueColor;

            nameFont = FilePaths.resources_font + reference.nameFont.name;
            dialogueFont = FilePaths.resources_font + reference.dialogueFont.name;

            nameFontScale = reference.nameFontScale;
            dialogueFontScale = reference.dialogueFontScale;
        }
    }

    public static List<CharacterData> Capture()
    {
        List<CharacterData> characters = new List<CharacterData>();

        foreach (var character in R.CharacterSystem.allCharacters)
        {
            if (!character.isVisible)
                continue;

            CharacterData entry = new CharacterData();
            entry.characterName = character.Name;
            entry.castingName = character.castingName;
            entry.displayName = character.displayName;
            entry.enabled = character.isVisible;
            entry.color = character.color;
            entry.priority = character.priority;
            entry.isFacingLeft = character.isFacingLeft;
            entry.isHighlighted = character.highlighted;
            entry.position = character.targetPosition;
            entry.characterConfig = new CharacterConfigCache(character.Config);
            entry.animationJSON = GetAnimationData(character);

            switch (character.Config.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    SpriteData sData = new SpriteData();
                    sData.layers = new List<SpriteData.LayerData>();

                    Character_Sprite sc = character as Character_Sprite;
                    foreach (var layer in sc.Layers)
                    {
                        var layerData = new SpriteData.LayerData();
                        layerData.color = layer.renderer.color;
                        layerData.spriteName = layer.renderer.sprite.name;
                        sData.layers.Add(layerData);
                    }

                    entry.dataJSON = JsonUtility.ToJson(sData);
                    break;
                case Character.CharacterType.Live2D:
                case Character.CharacterType.Model3D:
                    break;
            }

            characters.Add(entry);
        }

        return characters;
    }

    public static void Apply(List<CharacterData> data)
    {
        List<string> cache = new List<string>();

        foreach (CharacterData characterData in data)
        {
            Character character = null;
            CharacterSystem characterSystem = R.CharacterSystem;

            if (characterData.castingName == string.Empty)
            {
                character = characterSystem.GetCharacter(characterData.characterName, createIfDoesNotExist: true);
            }
            else
            {
                character = characterSystem.GetCharacter(characterData.characterName, createIfDoesNotExist: false);

                if (character == null)
                {
                    string castingName = $"{characterData.characterName}{CharacterSystem.CHARACTER_CASTING_ID}{characterData.castingName}";
                    character = characterSystem.CreateCharacter(castingName);
                }
            }

            character.DisplayName = characterData.displayName;
            character.SetColor(characterData.color);

            if (characterData.isHighlighted)
                character.Highlight(immediate: true);
            else
                character.UnHighlight(immediate: true);

            character.SetPriority(characterData.priority);

            if (characterData.isFacingLeft)
                character.FaceLeft(immediate: true);
            else
                character.FaceRight(immediate: true);

            character.SetPosition(characterData.position);

            character.isVisible = characterData.enabled;

            AnimationData animationData = JsonUtility.FromJson<AnimationData>(characterData.animationJSON);
            ApplyAnimationData(character, animationData);

            switch (character.Config.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    SpriteData sData = JsonUtility.FromJson<SpriteData>(characterData.dataJSON);
                    Character_Sprite sc = character as Character_Sprite;

                    for (int i = 0; i < sData.layers.Count; i++)
                    {
                        var layer = sData.layers[i];
                        if (sc.Layers[i].renderer.sprite != null && sc.Layers[i].renderer.sprite.name != layer.spriteName)
                        {
                            Sprite sprite = sc.GetSprite(layer.spriteName);
                            if (sprite != null)
                                sc.SetSprite(sprite, i);
                            else
                                Debug.LogWarning($"历史状态无法加载精灵 '{layer.spriteName}'");
                        }
                    }

                    break;
                case Character.CharacterType.Live2D:
                   
                    break;
                case Character.CharacterType.Model3D:
                   
                    break;
            }

            cache.Add(character.Name);
        }

        foreach (Character character in  R .CharacterSystem.allCharacters)
        {
            if (!cache.Contains(character.Name))
                character.isVisible = false;
        }
    }

    private static string GetAnimationData(Character character)
    {
        Animator animator = character.Animator;
        AnimationData data = new AnimationData();

        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                continue;

            AnimationParameter pData = new AnimationParameter { name = param.name };

            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    pData.type = "Bool";
                    pData.value = animator.GetBool(param.name).ToString();
                    break;
                case AnimatorControllerParameterType.Float:
                    pData.type = "Float";
                    pData.value = animator.GetFloat(param.name).ToString();
                    break;
                case AnimatorControllerParameterType.Int:
                    pData.type = "Int";
                    pData.value = animator.GetInteger(param.name).ToString();
                    break;
            }

            data.parameters.Add(pData);
        }

        return JsonUtility.ToJson(data);
    }

    private static void ApplyAnimationData(Character character, AnimationData data)
    {
        Animator animator = character.Animator;

        foreach (var param in data.parameters)
        {
            switch (param.type)
            {
                case "Bool":
                    animator.SetBool(param.name, bool.Parse(param.value));
                    break;
                case "Float":
                    animator.SetFloat(param.name, float.Parse(param.value));
                    break;
                case "Int":
                    animator.SetInteger(param.name, int.Parse(param.value));
                    break;
            }
        }

        animator.SetTrigger(Character.ANIMATION_REFRESH_TRIGGER);
    }

    [System.Serializable]
    public class AnimationData
    {
        public List<AnimationParameter> parameters = new List<AnimationParameter>();

        [System.Serializable]
        public class AnimationParameter
        {
            public string name;
            public string type;
            public string value;
        }
    }

    [System.Serializable]
    public class SpriteData
    {
        public List<LayerData> layers;

        [System.Serializable]
        public class LayerData
        {
            public string spriteName;
            public Color color;
        }
    }

    [System.Serializable]
    public class Live2DData
    {
        public string expression;
        public string motion;
    }

    [System.Serializable]
    public class Model3DData
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}