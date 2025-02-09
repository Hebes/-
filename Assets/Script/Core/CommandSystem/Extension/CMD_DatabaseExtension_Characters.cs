using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 数据库扩展字符-角色
/// </summary>
public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
{
    private static string[] PARAM_ENABLE => new string[] { "-e", "-enable" };
    private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
    private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
    private static string[] PARAM_SMOOT => new string[] { "-sm", "-smooth" };
    private static string PARAM_XPOS => "-x";
    private static string PARAM_YPOS => "-y";
    private static string[] PARAM_ANIM => new string[] { "-a", "-anim", "-animation" };
    private static string[] PARAM_STATE => new string[] { "-s", "-state" };

    private static char EXPRESSIONLAYER_DELIMITER => DL_SPEAKER_DATA.EXPRESSIONLAYER_DELIMITER;
    private static char EXPRESSIONLAYER_JOINER => DL_SPEAKER_DATA.EXPRESSIONLAYER_JOINER;

    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("CreateCharacter", new Action<string[]>(CreateCharacter));
        database.AddCommand("MoveCharacter", new Func<string[], IEnumerator>(MoveCharacter));
        database.AddCommand("Show", new Func<string[], IEnumerator>(ShowAll));
        database.AddCommand("Hide", new Func<string[], IEnumerator>(HideAl1));
        database.AddCommand("Sort", new Action<string[]>(Sort));
        database.AddCommand("HighLight", new Func<string[], IEnumerator>(HighLightAll));
        database.AddCommand("UnHighLight", new Func<string[], IEnumerator>(UnHighLightAll));

        //添加角色命令
        CommandDataBase baseCommands = R.CommandSystem.CreateSubDatabase(CommandSystem.DATABASE_CHARACTERS_BASE);
        baseCommands.AddCommand("Move", new Func<string[], IEnumerator>(MoveCharacter));
        baseCommands.AddCommand("Show", new Func<string[], IEnumerator>(Show));
        baseCommands.AddCommand("Hide", new Func<string[], IEnumerator>(Hide));
        baseCommands.AddCommand("SetPriority", new Action<string[]>(SetPriority));
        baseCommands.AddCommand("SetPosition", new Action<string[]>(SetPosition));
        baseCommands.AddCommand("SetColor", new Func<string[], IEnumerator>(SetColor));
        baseCommands.AddCommand("Highlight", new Func<string[], IEnumerator>(HighLight));
        baseCommands.AddCommand("UnHighlight", new Func<string[], IEnumerator>(UnHighLight));
        baseCommands.AddCommand("FaceLeft", new Func<string[], IEnumerator>(FaceLeft));
        baseCommands.AddCommand("FaceRight", new Func<string[], IEnumerator>(FaceRight));
        baseCommands.AddCommand("Animate", new Action<string[]>(Animate));

        //添加特定字符的数据库
        CommandDataBase spriteCommands = R.CommandSystem.CreateSubDatabase(CommandSystem.DATABASE_CHARACTERS_SPRITE);
        spriteCommands.AddCommand("SetSprite", new Func<string[], IEnumerator>(SetSprite));
    }


    #region Global Commands
    private static void Animate(string[] data)
    {
        string characterName = data[0];
        Character character = R.CharacterSystem.GetCharacter(characterName);

        if (character == null)
        {
            Debug.LogError($"没有角色 '{data[0]}' 被发现. 没有动画.");
            return;
        }

        string animation;
        bool state;

        var parameters = ConvertDataToParameters(data, 1);
        parameters.TryGetValue(PARAM_ANIM, out animation); //获取动画速度
        bool hasState = parameters.TryGetValue(PARAM_STATE, out state, defaultValue: true); //获取状态

        if (hasState)
            character.Animate(animation, state);
        else
            character.Animate(animation);
    }
    private static IEnumerator FaceRight(string[] data)
    {
        yield return FaceDirection(left: false, data);
    }
    private static IEnumerator FaceLeft(string[] data)
    {
        yield return FaceDirection(left: true, data);
    }
    private static IEnumerator UnHighLightAll(string[] data)
    {
        bool immediate = false;
        bool handleUnspecifiedCharacters = true;
        List<Character> characters = new List<Character>();
        List<Character> unspecifiedcharacters = new List<Character>();

        //添加指定要突出显示的任何字符.
        if (data[0].ToLower() == "all")
        {
            characters.AddRange(R.CharacterSystem.allCharacters);
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }
        }

        if (characters.Count == 0)
            yield break;

        var parameters = ConvertDataToParameters(data, startingIndex: 1); //获取额外的参数

        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);
        parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedCharacters, defaultValue: true);

        //使所有字符执行逻辑
        foreach (Character character in characters)
            character.Highlight(immediate: immediate);

        //如果我们强迫任何未指定的字符使用相反的突出显示状态
        if (handleUnspecifiedCharacters)
        {
            foreach (Character character in R.CharacterSystem.allCharacters)
            {
                if (characters.Contains(character))
                    continue;
                unspecifiedcharacters.Add(character);
                character.UnHighlight(immediate: immediate);
            }
        }

        //等待所有字符关闭高亮
        if (!immediate)
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() =>
            {
                foreach (var character in characters)
                    character.Highlight(immediate: true);

                if (!handleUnspecifiedCharacters)
                    return;

                foreach (var character in unspecifiedcharacters)
                    character.UnHighlight(immediate: true);
            });

            bool temp = handleUnspecifiedCharacters & unspecifiedcharacters.Any(uc => uc.isHighlighting);
            while (characters.Any(c => c.isUnHighlighting) || temp)
                yield return null;
        }
    }
    private static IEnumerator HighLightAll(string[] data)
    {
        // 13.4
        bool immediate = false;
        bool handleUnspecifiedcharacters = true;
        List<Character> characters = new List<Character>();
        List<Character> unspecifiedcharacters = new List<Character>();

        if (data[0].ToLower() == "all")
        {
            characters.AddRange(R.CharacterSystem.allCharacters);
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }
        }

        if (characters.Count == 0)
            yield break;

        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);
        parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedcharacters, defaultValue: true);

        foreach (Character character in characters)
            character.Highlight(immediate: immediate);

        if (handleUnspecifiedcharacters)
        {
            foreach (Character character in R.CharacterSystem.allCharacters)
            {
                if (characters.Contains(character))
                    continue;
                unspecifiedcharacters.Add(character);
                character.UnHighlight(immediate: immediate);
            }
        }

        //等待所有字符完成高亮
        if (!immediate)
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() =>
            {
                foreach (var character in characters)
                    character.Highlight(immediate: true);

                if (!handleUnspecifiedcharacters)
                    return;

                foreach (var character in unspecifiedcharacters)
                    character.UnHighlight(immediate: true);
            });

            while (characters.Any(c => c.isHighlighting) || handleUnspecifiedcharacters & unspecifiedcharacters.Any(uc => uc.isUnHighlighting))
                yield return null;
        }
    }
    private static void Sort(string[] data)
    {
        R.CharacterSystem.SortCharacters(data);
    }
    private static void CreateCharacter(string[] data)
    {
        string characterName = data[0];
        var parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_ENABLE, out bool enable, defaultValue: false);
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        Character character = R.CharacterSystem.CreateCharacter(characterName);

        if (!enable)
            return;

        if (immediate)
            character.isVisible = true;
        else
            character.Show();
    }
    private static IEnumerator HideAl1(string[] data)
    {
        List<Character> characters = new List<Character>();
        bool immediate = false;
        if (data[0].ToLower() == "all")
        {
            characters.AddRange(R.CharacterSystem.allCharacters);
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }
        }

        if (characters.Count == 0)
            yield break;

        CommandParameters parameters = ConvertDataToParameters(data); //将数据数组转换为参数容器

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);

        //调用所有角色的逻辑
        foreach (Character character in characters)
        {
            if (immediate)
                character.isVisible = false;
            else
                character.Hide(speed);
        }


        if (!immediate)
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() =>
            {
                foreach (Character character in characters)
                    character.isVisible = false;
            });

            while (characters.Any(c => c.co_hiding.Has()))
                yield return null;
        }
    }
    private static IEnumerator ShowAll(string[] data)
    {
        List<Character> characters = new List<Character>();
        bool immediate = false;
        float speed = 1;
        if (data[0].ToLower() == "all")
        {
            characters.AddRange(R.CharacterSystem.allCharacters);
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }
        }

        if (characters.Count == 0)
            yield break;

        //将数据数组转换为参数容器
        CommandParameters parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

        //调用所有角色的逻辑
        foreach (Character character in characters)
        {
            if (immediate)
                character.isVisible = true;
            else
                character.Show(speed);
        }


        if (!immediate)
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() =>
            {
                foreach (Character character in characters)
                    character.isVisible = true;
            });

            while (characters.Any(c => c.co_hiding.Has()))
                yield return null;
        }
    }
    private static IEnumerator MoveCharacter(string[] data)
    {
        string characterName = data[0];
        Character character = R.CharacterSystem.GetCharacter(characterName);
        if (character == null)
            yield break;
        CommandParameters parameters = ConvertDataToParameters(data);

        //得到x轴的位置
        parameters.TryGetValue(PARAM_XPOS, out float x, defaultValue: 0);

        //得到y轴的位置
        parameters.TryGetValue(PARAM_YPOS, out float y, defaultValue: 0);
        //获得速度
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1);
        //平滑
        parameters.TryGetValue(PARAM_SMOOT, out bool smooth, defaultValue: false);
        //立即设定位置
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        Vector2 position = new Vector2(x, y);
        if (immediate)
            character.SetPosition(position);
        else
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.SetPosition(position); });
            yield return character.MoveToPosition(position, speed, smooth);
        }
    }
    #endregion


    #region BASE
    private static void SetPosition(string[] data)
    {
        Character character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false);

        if (character == null || data.Length < 2)
            return;

        var parameters = ConvertDataToParameters(data, 1);

        parameters.TryGetValue(PARAM_XPOS, out float x, defaultValue: 0);
        parameters.TryGetValue(PARAM_YPOS, out float y, defaultValue: 0);

        character.SetPosition(new Vector2(x, y));
    }
    private static void SetPriority(string[] data)
    {
        Character character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false);

        if (character == null || data.Length < 2)
            return;

        if (!int.TryParse(data[1], out int priority))
            priority = 0;
        character.SetPriority(priority);
    }
    private static IEnumerator Hide(string[] data)
    {
        Character character = R.CharacterSystem.GetCharacter(data[0]);

        if (character == null)
            yield break;

        var parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out bool immediate, defaultValue: false);

        if (immediate)
            character.isVisible = false;
        else
        {
            //长时间运行的流程应该有一个停止操作来取消协程，并运行应该完成此逗号的逻辑
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character.isVisible = false; });
            yield return character.Hide();
        }
    }
    private static IEnumerator Show(string[] data)
    {
        Character character = R.CharacterSystem.GetCharacter(data[0]);

        if (character == null)
            yield break;

        var parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out bool immediate, defaultValue: false);

        if (immediate)
            character.isVisible = true;
        else
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character.isVisible = true; });
            yield return character.Show();
        }
    }
    private static IEnumerator SetColor(string[] data)
    {
        Character character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false);
        string colorName;
        float speed;
        bool immediate;
        if (character == null || data.Length < 2)
            yield break;

        var parameters = ConvertDataToParameters(data, startingIndex: 1); //获取额外的参数
        parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName); //获取颜色名称
        bool specifiedSpeed = parameters.TryGetValue(new string[] { "-spd", "-speed" }, out speed, defaultValue: 1f); //获取转换的速度
        if (!specifiedSpeed)
            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: true); //获取值
        else
            immediate = false;
        //获取名字颜色 
        Color color = Color.white;
        color = color.GetColorFromName(colorName);
        if (immediate)
        {
            character.SetColor(color);
        }
        else
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
            character.TransitionColor(color, speed);
        }

        yield break;
    }
    private static IEnumerator UnHighLight(string[] data)
    {
        //format: SetSprite(character sprite)
        Character character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false) as Character;
        if (character == null)
            yield break;
        //Grab the extra parameters
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out bool immediate, defaultValue: false);
        if (immediate)
            character.UnHighlight(immediate: true);
        else
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate: true); });
        yield return character.UnHighlight();
    }
    private static IEnumerator HighLight(string[] data)
    {
        //format: SetSprite(character sprite)
        Character character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false) as Character;
        if (character == null)
            yield break;
        //Grab the extra parameters
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out bool immediate, defaultValue: false);
        if (immediate)
            character.Highlight(immediate: true);
        else
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate: true); });
        yield return character.Highlight();
    }
    #endregion

    
    #region Sprite
    private static IEnumerator SetSprite(string[] data)
    {
        // 13.4
        //format: SetSprite(character sprite)
        Character_Sprite character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;
        int layer = 0;
        string spriteName;
        bool immediate = false;
        float speed;

        if (character == null || data.Length < 2)
            yield break;

        var parameters = ConvertDataToParameters(data, startingIndex: 1); //获取额外的参数
        parameters.TryGetValue(new[] { "-s", "-sprite" }, out spriteName); //试着获取名称
        parameters.TryGetValue(new[] { "-l", "-layer" }, out layer, defaultValue: 0); //试着获取层级
        bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f); //试着获得过渡速度

        if (!specifiedSpeed)
        {
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false); //是否是一个直接的过渡
        }

        List<(Sprite sprite, int layer)> assignments = new List<(Sprite sprite, int layer)>();

        if (spriteName.Contains(EXPRESSIONLAYER_DELIMITER) || spriteName.Contains(EXPRESSIONLAYER_JOINER))
        {
            int layerCounter = 0;
            foreach (string pair in spriteName.Split(EXPRESSIONLAYER_JOINER))
            {
                string[] v = pair.Split(EXPRESSIONLAYER_DELIMITER);
                Sprite pairSprite = null;
                int pairLayer = 0;

                //如果我们没有为精灵指定一个图层，我们可以在递增的图层上设置多个精灵
                if (v.Length < 2)
                {
                    pairSprite = character.GetSprite(v[0]);
                    pairLayer = layerCounter++;
                }
                else
                {
                    pairSprite = character.GetSprite(v[1]);
                    pairLayer = 0;
                    if (!int.TryParse(v[0], out pairLayer))
                        pairLayer = layerCounter++;
                }

                assignments.Add((pairSprite, pairLayer));
            }
        }
        else
        {
            assignments.Add((character.GetSprite(spriteName), layer));
        }

        //添加一个可以在每个表达式层引用上运行的终止命令
        R.CommandSystem.AddTerminationActionToCurrentProcess(
            () =>
            {
                foreach ((Sprite sprite, int layer) pair in assignments)
                {
                    character.SetSprite(pair.sprite, pair.layer);
                }
            }
        );

        //在每个表达式和层对上运行逻辑
        Coroutine retVal = null;
        foreach ((Sprite sprite, int layer) pair in assignments)
        {
            Sprite sprite = pair.sprite;
            layer = pair.layer;

            if (sprite == null)
                continue;

            if (immediate)
            {
                character.SetSprite(sprite, layer);
            }
            else
            {
                retVal = character.TransitionSprite(sprite, layer, speed);
            }
        }

        if (!immediate)
            yield return retVal;
    }
    #endregion

    
    private static IEnumerator FaceDirection(bool left, string[] data)
    {
        string characterName = data[0];
        Character character = R.CharacterSystem.GetCharacter(characterName);

        if (character == null)
            yield break;

        var parameters = ConvertDataToParameters(data);

        //Try to get the speed of the flip
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);

        //Try to see if this is an immediate effect or not.
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        if (left)
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.FaceLeft(immediate: true); });
            yield return character.FaceLeft(speed, immediate);
        }
        else
        {
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.FaceRight(immediate: true); });
            yield return character.FaceRight(speed, immediate);
        }
    }
}