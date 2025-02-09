using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音效系统
/// </summary>
public class AudioSystem : SM<AudioSystem>
{
    public AudioMixer MianAudioMixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voicesMixer;
    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();
    public AnimationCurve audioFalloffCurve;

    private Transform sfxRoot;
    public static char[] SFX_NAME_FORMAT_CONTAINERS = new char[] { '[', ']' };
    private const string SFX_NAME_FORMAT = "SFX-[{0}]";
    public const float TRACK_TRANSITION_SPEED = 1f;
    public const string MUSIC_VOLUME_PARAMETER_NAME = "MusicVolume";
    public const string SFX_VOLUME_PARAMETER_NAME = "SFXVolume";
    public const string VOICES_VOLUME_PARAMETER_NAME = "VoicesVolume";
    public const float MUTED_VOLUME_LEVEL = -80f;
    public AudioSource[] allSFX => sfxRoot.GetComponentsInChildren<AudioSource>();

    private void Awake()
    {
        MianAudioMixer = R.Load<AudioMixer>("Audio/Mian");

        AudioMixerGroup[] audioMixerGroups = MianAudioMixer.FindMatchingGroups("Master");
        foreach (AudioMixerGroup audioMixerGroup in audioMixerGroups)
        {
            if ("Music".Equals(audioMixerGroup.name))
            {
                musicMixer = audioMixerGroup;
            }
            else if ("SFX".Equals(audioMixerGroup.name))
            {
                sfxMixer = audioMixerGroup;
            }
            else if ("Voices".Equals(audioMixerGroup.name))
            {
                voicesMixer = audioMixerGroup;
            }
        }

        sfxRoot = new GameObject(musicMixer.name).transform;
        sfxRoot.SetParent(transform);

        audioFalloffCurve = new AnimationCurve();
        audioFalloffCurve.AddKey(0f, -80f); // 在时间0处，值为0
        audioFalloffCurve.AddKey(0.2f, -25f); // 在时间0处，值为0
        audioFalloffCurve.AddKey(0.5f, -10f); // 在时间0处，值为0
        audioFalloffCurve.AddKey(1f, 0f); // 在时间0处，值为0
    }

    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = R.Load<AudioClip>(filePath);
        if (clip == null)
            Debug.LogError($"加载不到文件{filePath}';");
        return PlaySoundEffect(clip, mixer, volume, pitch, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;
        effectSource.clip = clip;
        if (mixer == null)
        {
            mixer = sfxMixer;
        }

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0f;
        effectSource.pitch = pitch;
        effectSource.loop = loop;

        effectSource.Play();

        if (!loop)
        {
            Destroy(effectSource.gameObject, clip.length / pitch + 1);
        }

        return effectSource;
    }

    public AudioSource PlayVoice(string filePath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filePath, voicesMixer, volume, pitch, loop);
    }

    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, voicesMixer, volume, pitch, loop);
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip.name.ToLower().Equals(soundName))
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1)
    {
        AudioClip clip = R.Load<AudioClip>(filePath);
        if (clip == null)
        {
            $"加载不到音乐文件'{filePath}'.请检查文件是否存在资源目录!".LogError();
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath);
    }

    /// <summary>
    /// 15.4 播放轨道
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="channel"></param>
    /// <param name="loop"></param>
    /// <param name="startingVolume"></param>
    /// <param name="volumeCap"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
        return track;
    }

    public void StopTrack(int channel)
    {
        AudioChannel c = TryGetChannel(channel, createIfDoesNotExist: false);
        if (c == null)
            return;
        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();
        foreach (var channel in channels.Values)
        {
            if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public void StopAllSoundEffects()
    {
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            Destroy(source.gameObject);
        }
    }

    public void StopAllTracks()
    {
        foreach (AudioChannel channel in channels.Values)
            channel.StopTrack();
    }

    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false)
    {
        if (channels.TryGetValue(channelNumber, out AudioChannel channel))
            return channel;
        if (createIfDoesNotExist)
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }

    public bool IsPlayingSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip.name.ToLower() == soundName)
                return true;
        }

        return false;
    }


    public void SetMusicVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
        musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, volume);
    }

    public void SetSFXVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
        sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, volume);
    }

    public void SetVoicesVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
        voicesMixer.audioMixer.SetFloat(VOICES_VOLUME_PARAMETER_NAME, volume);
    }
}