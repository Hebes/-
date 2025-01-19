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

    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("CreateCharacter", new Action<string[]>(CreateCharacter));
        database.AddCommand("MoveCharacter", new Func<string[], IEnumerator>(MoveCharacter));
        database.AddCommand("Show", new Func<string[], IEnumerator>(ShowAll));
        database.AddCommand("hide", new Func<string[], IEnumerator>(HideAl1));
        database.AddCommand("highLight", new Func<string[], IEnumerator>(HighLightAll));
        database.AddCommand("unHighLight", new Func<string[], IEnumerator>(UnHighLightAll));

        //Addcommands to characters
        CommandDataBase baseCommands = R.CommandSystem.CreateSubDatabase(CommandSystem.DATABASE_CHARACTERS_BASE);
        baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
        baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
        baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
        baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
        baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
        baseCommands.AddCommand("setColor", new Func<string[], IEnumerator>(SetColor));
        baseCommands.AddCommand("highlight", new Func<string[], IEnumerator>(HighLight));
        baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnHighLight));
        baseCommands.AddCommand("faceleft", new Func<string[], IEnumerator>(FaceLeft));
        baseCommands.AddCommand("faceright", new Func<string[], IEnumerator>(FaceRight));
        baseCommands.AddCommand("animate", new Action<string[]>(Animate));

        //Add character specific databases
        CommandDataBase spriteCommands = R.CommandSystem.CreateSubDatabase(CommandSystem.DATABASE_CHARACTERS_SPRITE);
        spriteCommands.AddCommand("setSprite", new Func<string[], IEnumerator>(SetSprite));
    }

    private static void Animate(string[] data)
    {
        string characterName = data[0];
        Character character = R.CharacterSystem.GetCharacter(characterName);

        if (character == null)
        {
            Debug.LogError($"No character called '{data[0]}' was found. Can not animate.");
            return;
        }

        string animation;
        bool state;

        var parameters = ConvertDataToParameters(data, 1);

        //Try to get the speed of the flip
        parameters.TryGetValue(PARAM_ANIM, out animation);

        //Try to see if this is an immediate effect or not.
        bool hasState = parameters.TryGetValue(PARAM_STATE, out state, defaultValue: true);

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


    private static IEnumerator UnHighLightAll(string[] data)
    {
        List<Character> characters = new List<Character>();
        List<Character> unspecifiedcharacters = new List<Character>();
        //Add any characters specified to-be-highlighted.
        for (int i = 0; i < data.Length; i++)
        {
            Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
            if (character != null)
                characters.Add(character);
        }

        if (characters.Count == 0)
            yield break;
        //Grab the-extra-parameters
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out bool immediate, defaultValue: false);
        parameters.TryGetValue(new string[] { "-o", "-only" }, out bool handleUnspecifiedcharacters, defaultValue: true);
        //Make all characters perform-the-logic
        foreach (Character character in characters)
            character.Highlight(immediate: immediate);
        //If we are forcing any unspecified characters to use the opposite-highlighted status
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

        //Wait for all characters to finish highlighting
        if (immediate)
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

            while (characters.Any(c => c.isUnHighlighting) || handleUnspecifiedcharacters & unspecifiedcharacters.Any(uc => uc.isHighlighting))
                yield return null;
        }
    }

    /// <summary>
    /// 13.4
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static IEnumerator HighLightAll(string[] data)
    {
        List<Character> characters = new List<Character>();
        bool immediate = false;
        bool handleUnspecifiedcharacters = true;
        List<Character> unspecifiedcharacters = new List<Character>();
        //Add any characters specified to-be-highlighted.
        for (int i = 0; i < data.Length; i++)
        {
            Character character = R.CharacterSystem.GetCharacter(data[i], createIfDoesNotExist: false);
            if (character != null)
                characters.Add(character);
        }

        if (characters.Count == 0)
            yield break;
        //Grab the-extra-parameters
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);
        parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedcharacters, defaultValue: true);
        //Make all characters perform-the-logic
        foreach (Character character in characters)
            character.Highlight(immediate: immediate);
        //If we are forcing any unspecified characters to use the opposite-highlighted status
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

        //Wait for all characters to finish highlighting
        if (immediate)
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

        if (!enable) return;
        if (immediate)
            character.isVisible = true;
        else
            character.Show();
    }

    private static IEnumerator HideAl1(string[] data)
    {
        List<Character> characters = new List<Character>();
        bool immediate = false;
        foreach (string s in data)
        {
            Character character = R.CharacterSystem.GetCharacter(s, createIfDoesNotExist: false);
            if (character != null)
                characters.Add(character);
        }

        if (characters.Count == 0)
            yield break;
        //将数据数组转换为参数容器
        CommandParameters parameters = ConvertDataToParameters(data);
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
        foreach (string s in data)
        {
            Character character = R.CharacterSystem.GetCharacter(s, createIfDoesNotExist: false);
            if (character != null)
                characters.Add(character);
        }

        if (characters.Count == 0)
            yield break;
        //将数据数组转换为参数容器
        CommandParameters parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

        //调用所有角色的逻辑
        foreach (Character character in characters)
        {
            if (immediate)
                character.isVisible = true;
            else
                character.Show();
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

    #region BASE

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
            //A long running process should-have a stop action to cancel out the coroutine and run logic that should complete this commar
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
            //A long running process should-have a stop action to cancel out the coroutine and run logic that should complete this commar
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
        //获取额外的参数
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        //获取颜色名称
        parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName);
        //获取转换的速度
        bool specifiedSpeed = parameters.TryGetValue(new string[] { "-spd", "-speed" }, out speed, defaultValue: 1f);
        //获取值
        if (!specifiedSpeed)
            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: true);
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

    /// <summary>
    /// 13.4
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static IEnumerator SetSprite(string[] data)
    {
        //format: SetSprite(character sprite)
        Character_Sprite character = R.CharacterSystem.GetCharacter(data[0], createIfDoesNotExist: false) as Character_Sprite;
        bool immediate = false;
        if (character == null || data.Length < 2)
            yield break;

        //Grab the extra parameters
        var parameters = ConvertDataToParameters(data, startingIndex: 1);
        //Try to get the sprite name
        parameters.TryGetValue(new string[] { "-s", "-sprite" }, out string spriteName);
        //Try to get the layer
        parameters.TryGetValue(new string[] { "-1", "-layer" }, out int layer, defaultValue: 0);
        //Try to get the transition speed
        bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 0.1f);
        //Try to get whether this is an immediate transition or not
        if (!specifiedSpeed)
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);
        //Run the logic
        Sprite sprite = character.GetSprite(spriteName);

        if (sprite == null)
            yield break;

        if (immediate)
            character.SetSprite(sprite, layer);
        else
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { character?.SetSprite(sprite, layer); });
        yield return character.TransitionSprite(sprite, layer, speed);
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