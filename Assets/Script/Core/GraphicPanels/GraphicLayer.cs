using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GraphicLayer
{
    public int layerDepth = 0;
    public const string LAYER_OBJECT_NAME_FORMAT = "Layer: {0}";

    public Transform panel;
    public GraphicObject currentGraphic = null;
    public List<GraphicObject> oldGraphics = new List<GraphicObject>();

    public Coroutine SetTexture(string filePath, float transitionSpeed = 1f, Texture blendingTexture = null, bool immediate = false)
    {
        Texture tex = R.Load<Texture>(filePath);
        if (tex == null)
        {
            Debug.LogError($"无法从路径加载图形纹理.{filePath}'请确保它存在于资源中!");
            return null;
        }

        return SetTexture(tex, transitionSpeed, blendingTexture, filePath);
    }

    public Coroutine SetTexture(Texture tex, float transitionSpeed = 1f, Texture blendingTexture = null, string filePath = "", bool immediate = false)
    {
        return CreateGraphic(tex, transitionSpeed, filePath, blendingTexture: blendingTexture, immediate: immediate);
    }

    #region Video

    public Coroutine SetVideo(string filePath, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, bool immediate = false)
    {
        VideoClip clip = R.Load<VideoClip>(filePath);
        if (clip == null)
        {
            $"无法从路径加载图形视频{filePath}.请确保它存在于资源中!".LogError();
            return null;
        }

        return SetVideo(clip, transitionSpeed, useAudio, blendingTexture, filePath);
    }

    public Coroutine SetVideo(VideoClip video, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, string filePath = "", bool immediate = false)
    {
        return CreateGraphic(video, transitionSpeed, filePath, useAudio, blendingTexture, immediate);
    }

    #endregion

    private Coroutine CreateGraphic<T>(T graphicData, float transitionSpeed, string filePath, bool useAudioForVideo = true, Texture blendingTexture = null, bool immediate = false)
    {
        GraphicObject newGraphic = default;
        switch (graphicData)
        {
            case Texture texture:
                newGraphic = new GraphicObject(this, filePath, texture, immediate);
                break;
            case VideoClip video:
                newGraphic = new GraphicObject(this, filePath, video, useAudioForVideo, immediate);
                break;
        }

        if (currentGraphic != null && !oldGraphics.Contains(currentGraphic))
            oldGraphics.Add(currentGraphic);
        currentGraphic = newGraphic;
        if (!immediate)
        {
            return currentGraphic.FadeIn(transitionSpeed, blendingTexture);
        }

        DestroyOldGraphics(); //立即生效
        return null;
    }

    public void DestroyOldGraphics()
    {
        foreach (GraphicObject g in oldGraphics)
            UnityEngine.Object.Destroy(g.Renderer.gameObject);
        oldGraphics.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transitionSpeed"></param>
    /// <param name="blendTexture"></param>
    /// <param name="immediate">是否立刻</param>
    public void Clear(float transitionSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        if (currentGraphic != null)
        {
            if (immediate)
                currentGraphic.Destroy();
            else
                currentGraphic.FadeOut(transitionSpeed, blendTexture);
        }
       
        for(int i = oldGraphics.Count - 1; i >= 0; i--)
        //foreach (GraphicObject g in oldGraphics)
        {
            GraphicObject g = oldGraphics[i];
            if (immediate)
                g.Destroy();
            else
                g.FadeOut(transitionSpeed, blendTexture);
        }
    }
}