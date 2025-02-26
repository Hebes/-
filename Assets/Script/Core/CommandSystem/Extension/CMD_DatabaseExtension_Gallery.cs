﻿using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 画廊
/// </summary>
public class CMD_DatabaseExtension_Gallery : CMD_DatabaseExtension
{
    private static string[] PARAM_MEDIA = new string[] { "-m", "-media" };
    private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
    private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
    private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };

    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("ShowGalleryImage", new Func<string[], IEnumerator>(ShowGalleryImage));
        database.AddCommand("HideGalleryImage", new Func<string[], IEnumerator>(HideGalleryImage));
    }

    public static IEnumerator HideGalleryImage(string[] data)
    {
        GraphicLayer graphicLayer = R.GraphicPanelSystem.GetPanel("Cinematic").GetLayer(0, createIfDoesNotExist: true);

        if (graphicLayer.currentGraphic == null)
            yield break;

        float transitionSpeed = 0;
        bool immediate = false;
        string blendTexName = "";
        Texture blendTex = null;

        var parameters = ConvertDataToParameters(data);//得到参数
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);//尝试获取值
        if (!immediate)
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1); //获取过度
        parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);//获取纹理

        if (!immediate && blendTexName != string.Empty)
            blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

        if (!immediate)
            R.CommandSystem.AddTerminationActionToCurrentProcess(() =>
            {
                Debug.Log("清空");
                graphicLayer.Clear(immediate: true);
            });

        graphicLayer.Clear(transitionSpeed, blendTex, immediate);

        if (graphicLayer.currentGraphic != null)
        {
            var graphicObject = graphicLayer.currentGraphic;

            yield return new WaitUntil(() => graphicObject == null);
        }
    }

    public static IEnumerator ShowGalleryImage(string[] data)
    {
        string mediaName = "";
        float transitionSpeed = 0;
        bool immediate = false;
        string blendTexName = "";
        Texture blendTex = null;

        
        var parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_MEDIA, out mediaName, defaultValue: "");
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        if (!immediate)
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);
        parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

        string pathToGraphic = FilePaths.resources_gallery + mediaName;
        Texture graphic = Resources.Load<Texture>(pathToGraphic);

        if (graphic == null)
        {
            Debug.LogError(
                $"找不到画廊图片 '{mediaName}' 在资源文件夹 '{FilePaths.resources_gallery}' 目录. 请指定资源中的完整路径，并确保该文件存在!");
            yield break;
        }

        if (!immediate && blendTexName != string.Empty)
            blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

        GraphicLayer graphicLayer = R.GraphicPanelSystem.GetPanel("Cinematic").GetLayer(0, createIfDoesNotExist: true);

        if (!immediate)
            R.CommandSystem.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetTexture(graphic, filePath: pathToGraphic, immediate: true); });

        GalleryConfig.UnlockImage(mediaName);

        yield return graphicLayer.SetTexture(graphic, transitionSpeed, blendTex, pathToGraphic, immediate);
    }
}