using UnityEngine;
using UnityEngine.Audio;

public class AudioTrack
{
    private const string TRACK_NAME_FORMAT = "Track-[{0}]";
    public string name;
    public string path { get; private set; }

    private AudioChannel channel;
    private AudioSource source;
    public bool Loop => source.loop;

    public float VolumeCap { get; private set; }
    public GameObject Root => source.gameObject;
    public bool IsPlaying => source.isPlaying;

    public float pitch
    {
        get => source.pitch;
        set => source.pitch = value;
    }

    public float Volume
    {
        get => source.volume;
        set => source.volume = value;
    }

    public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, AudioChannel channel, AudioMixerGroup mixer, string filePath)
    {
        name = clip.name;
        path = filePath;

        this.channel = channel;
        VolumeCap = volumeCap;

        source = CreateSource();
        source.clip = clip;
        source.loop = loop;
        source.volume = startingVolume;
        source.pitch = pitch;

        source.outputAudioMixerGroup = mixer;
    }

    private AudioSource CreateSource()
    {
        GameObject go = new GameObject(string.Format(TRACK_NAME_FORMAT, name));
        go.transform.SetParent(channel.trackContainer);
        return go.AddComponent<AudioSource>();
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}