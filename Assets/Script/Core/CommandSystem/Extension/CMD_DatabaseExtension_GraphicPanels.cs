using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CMD_DatabaseExtension_GraphicPanels : CMD_DatabaseExtension
{
    private static string[] PARAM_PANEL = new string[] { "-p", "-panel" };
    private static string[] PARAM_LAYER = new String[] { "-l", "-layer" };
    private static string[] PARAM_MEDIA = new String[] { "-m", "-media" };
    private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
    private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
    private static string[] PARAM_BLENDTEX = new String[] { "-b", "-blend" };
    private static string[] PARAM_USEVIDEOAUDIO = new string[] { "-aud", "-audio" };
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("SetLayerMedia", new Func<string[], IEnumerator>(SetLayerMedia));
        database.AddCommand("ClearLayerMedia", new Func<string[], IEnumerator>(ClearLayerMedia));
    }

    private static IEnumerator ClearLayerMedia(string[] data)
    {
        //Parameters available to function
        float transitionSpeed = 0;
        bool useAudio = false;

        string pathToGraphic = "";
        UnityEngine.Object graphic = null;
        Texture blendTex = null;

        //Now get the parameters
        CommandParameters parameters = ConvertDataToParameters(data);

        //Try to get the panel that this media is applied to
        parameters.TryGetValue(PARAM_PANEL, out string panelName);
        GraphicPanel panel = R.GraphicPanelSystem.GetPanel(panelName);
        if (panel == null)
        {
            Debug.LogError($"无法抓取面板.'{panelName}'因为这不是一个有效的小组。请检查面板名称并调整命令.");
            yield break;
        }

        //Try to get·the layer to apply this graphic to
        parameters.TryGetValue(PARAM_LAYER, out int layer, defaultValue: -1);
        //Try to get the graphic
        parameters.TryGetValue(PARAM_MEDIA, out string mediaName); //媒体名称
        //Try·to get·if this is·an immediate effect or·not
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);
        //Try to get the speed of the transition if it is not an immediate effect
        if (!immediate) parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);
        //Try to get the blending texture for the media if·we are using one.
        parameters.TryGetValue(PARAM_BLENDTEX, out string blendTexName);

        if (!immediate && blendTexName.IsNoNullOrNoEmpty())
        {
            blendTex = R.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);
        }

        if (layer == -1)
        {
            panel.Clear(transitionSpeed, blendTex, immediate);
        }
        else
        {
            GraphicLayer graphicLayer = panel.GetLayer(layer);
            if (graphicLayer == null)
                $"不能清层级 [{layer}] 没有面板 {panel.panelName}".LogError();
            else
                graphicLayer.Clear(transitionSpeed, blendTex, immediate);
        }
    }

    private static IEnumerator SetLayerMedia(string[] data)
    {
        //Parameters available to function
        string panelName = "";
        int layer = 0;
        string mediaName = "";
        bool immediate = false;
        float transitionSpeed = 0;
        string blendTexName = "";
        bool useAudio = false;

        string pathToGraphic = "";
        UnityEngine.Object graphic = null;
        Texture blendTex = null;

        //Now get the parameters
        CommandParameters parameters = ConvertDataToParameters(data);

        //Try to get the panel that this media is applied to
        parameters.TryGetValue(PARAM_PANEL, out panelName);
        GraphicPanel panel = R.GraphicPanelSystem.GetPanel(panelName);
        if (panel == null)
        {
            Debug.LogError($"无法抓取面板.'{panelName}'因为这不是一个有效的小组。请检查面板名称并调整命令.");
            yield break;
        }

        //Try to get·the layer to apply this graphic to
        parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);
        //Try to get the graphic
        parameters.TryGetValue(PARAM_MEDIA, out mediaName); //媒体名称
        //Try·to get·if this is·an immediate effect or·not
        parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
        //Try to get the speed of the transition if it is not an immediate effect
        if (!immediate) parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);
        //Try to get the blending texture for the media if·we are using one.
        parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);
        //If this is a video,try to get whether we use audio from the video or not
        parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio, defaultValue: false);
        //Now·run·the·logic
        pathToGraphic = GetPathToGraphic(FilePaths.resources_backgroundImages, mediaName);
        graphic = R.Load<Texture>(pathToGraphic);
        if (graphic == null)
        {
            pathToGraphic = GetPathToGraphic(FilePaths.resources_backgroundVideos, mediaName);
            graphic = R.Load<VideoClip>(pathToGraphic);
        }

        if (graphic == null)
        {
            Debug.LogError($"找不到被调用的媒体文件 {mediaName}在资源目录中. 请指定资源中的完整路径，并确保文件存在!");
            yield break;
        }

        if (!immediate && blendTexName.IsNoNullOrNoEmpty())
        {
            blendTex = R.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);
        }

        //Lets·try to get·the-layer to apply the media to
        GraphicLayer graphicLayer = panel.GetLayer(layer, createIfDoesNotExist: true);
        if (graphic is Texture texture)
            yield return graphicLayer.SetTexture(texture, transitionSpeed, blendTex, pathToGraphic, immediate);
        else
            yield return graphicLayer.SetVideo((VideoClip)graphic, transitionSpeed, useAudio, blendTex, pathToGraphic, immediate);
    }

    private static string GetPathToGraphic(string defaultPath, string graphicName)
    {
        if (graphicName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return graphicName.Substring(HOME_DIRECTORY_SYMBOL.Length);
        return defaultPath + graphicName;
    }
}