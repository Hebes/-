using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Object = UnityEngine.Object;

public class GraphicObject
{
    private const string NAME_FORMAT = "Graphic-[{0}]";
    private const string DEFAULT_UI_MATERIAL = "Default UI Material";
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    private const string MATERIAL_FIELD_COLOR = "_Color";
    private const string MATERIAL_FIELD_MAINTEX = "_MainTex";
    private const string MATERIAL_FIELD_BLENDTEX = "_BlendTex";
    private const string MATERIAL_FIELD_BLEND = "_Blend";
    private const string MATERIAL_FIELD_ALPHA = "_Alpha";

    public RawImage Renderer;
    public VideoPlayer video = null;
    public AudioSource audio = null;
    public string GraphicPath;
    private GraphicLayer layer;
    public bool IsVideo => video != null;
    public string graphicName { get; private set; }

    private Coroutine co_fadingIn = null;
    private Coroutine co_fadingOut = null;

    public bool useAudio => (audio != null && !audio.mute);

    public GraphicObject(GraphicLayer layer, string graphicPath, Texture tex, bool immediate)
    {
        this.GraphicPath = graphicPath;
        this.layer = layer;
        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        Renderer = ob.AddComponent<RawImage>();
        graphicName = tex.name;
        InitGraphic(immediate);
        Renderer.name = string.Format(NAME_FORMAT, graphicName);
        Renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
    }

    public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip, bool useAudio, bool immediate)
    {
        this.GraphicPath = graphicPath;
        this.layer = layer;
        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        Renderer = ob.AddComponent<RawImage>();
        graphicName = clip.name;
        Renderer.name = string.Format(NAME_FORMAT, graphicName);
        InitGraphic(immediate);

        RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
        Renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);

        video = ob.AddComponent<VideoPlayer>();
        video.playOnAwake = true;
        video.source = VideoSource.VideoClip;
        video.clip = clip;
        video.renderMode = VideoRenderMode.RenderTexture;
        video.targetTexture = tex;
        video.isLooping = true;
        video.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audio = ob.AddComponent<AudioSource>();
        audio.volume = 0;
        if (!useAudio)
            audio.mute = true;
        video.SetTargetAudioSource(0, audio);
        video.frame = immediate ? 1 : 0;
        video.Prepare();
        video.Play();
        video.enabled = true;
    }

    private void InitGraphic(bool immediate)
    {
        Renderer.transform.localPosition = Vector3.zero;
        Renderer.transform.localScale = Vector3.one;
        RectTransform rect = (RectTransform)Renderer.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        Renderer.material = GetTransitionMaterial();
        float startingOpacity = immediate ? 1.0f : 0.0f;
        Renderer.material.SetFloat(MATERIAL_FIELD_BLEND, startingOpacity);
        Renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, startingOpacity);
    }

    private Material GetTransitionMaterial()
    {
        Material mat = R.AssetLoadSystem.Load<Material>(MATERIAL_PATH);
        if (mat != null)
            return new Material(mat);
        return null;
    }

    public Coroutine FadeIn(float speed = 1f, Texture blend = null)
    {
        if (co_fadingOut.Has())
            R.StopCoroutine(co_fadingOut);
        if (co_fadingIn.Has())
            return co_fadingIn;
        co_fadingIn = R.StartCoroutine(Fading(1f, speed, blend));
        return co_fadingIn;
    }

    public Coroutine FadeOut(float speed = 1f, Texture blend = null)
    {
        if (co_fadingIn.Has())
            R.StopCoroutine(co_fadingIn);
        if (co_fadingOut.Has())
            return co_fadingOut;
        co_fadingOut = R.StartCoroutine(Fading(0f, speed, blend));
        return co_fadingOut;
    }


    private IEnumerator Fading(float target, float speed, Texture blend)
    {
        bool isBlending = blend != null;
        bool fadingIn = target > 0;

        if (DEFAULT_UI_MATERIAL.Equals(Renderer.material.name))
        {
            Texture tex = Renderer.material.GetTexture(MATERIAL_FIELD_MAINTEX);
            Renderer.material = GetTransitionMaterial();
            Renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
        }

        Renderer.material.SetTexture(MATERIAL_FIELD_BLENDTEX, blend);
        Renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, isBlending ? 1 : fadingIn ? 0 : 1);
        Renderer.material.SetFloat(MATERIAL_FIELD_BLEND, isBlending ? fadingIn ? 0 : 1 : 1);

        string opacityParam = isBlending ? MATERIAL_FIELD_BLEND : MATERIAL_FIELD_ALPHA;
        while (!Mathf.Approximately(Renderer.material.GetFloat(opacityParam), target))
        {
            float opacity = Mathf.MoveTowards(Renderer.material.GetFloat(opacityParam), target, speed * R.DeltaTime);
            Renderer.material.SetFloat(opacityParam, opacity);

            if (IsVideo)
                audio.volume = opacity;

            yield return null;
        }

        co_fadingIn = null;
        co_fadingOut = null;

        if (target == 0)
        {
            Destroy();
        }
        else
        {
            DestroyBackgroundGraphicsOnLayer();
            if (Renderer != null)
            {
                Renderer.texture = Renderer.material.GetTexture(MATERIAL_FIELD_MAINTEX);
                Renderer.material = null;
            }
        }
    }

    public void Destroy()
    {
        if (layer.currentGraphic != null && layer.currentGraphic.Renderer == Renderer)
            layer.currentGraphic = null;
        var oldGraphic = layer.oldGraphics.FirstOrDefault(g => g.Renderer == Renderer);
        if (oldGraphic != null)
            layer.oldGraphics.Remove(oldGraphic);

        if (Renderer != null)
            Object.Destroy(Renderer.gameObject);
    }

    private void DestroyBackgroundGraphicsOnLayer()
    {
        layer.DestroyOldGraphics();
    }
}