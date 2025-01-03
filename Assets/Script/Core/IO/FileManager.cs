using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    public static List<string> ReadTextFile(string filepath, bool includeBlankLines = true)
    {
        if (!filepath.StartsWith('/'))
            filepath = FilePaths.root + filepath;
        List<string> lines = new List<string>();
        try
        {
            using StreamReader sr = new StreamReader(filepath);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (includeBlankLines || !line.IsNullOrEmpty())
                    lines.Add(line);
            }
        }
        catch (Exception e)
        {
            throw new Exception("文件未找到");
        }

        return lines;
    }

    public static List<string> ReadTextAsset(string filepath, bool includeBlankLines = true)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filepath);
        if (textAsset == null)
        {
            throw new Exception("文件未找到");
        }

        return ReadTextAsset(textAsset, includeBlankLines);
    }

    public static List<string> ReadTextAsset(TextAsset textAsset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using StringReader sr = new StringReader(textAsset.text);
        while (sr.Peek() > -1)
        {
            string line = sr.ReadLine();
            if (includeBlankLines || !line.IsNullOrEmpty())
                lines.Add(line);
        }

        return lines;
    }
}