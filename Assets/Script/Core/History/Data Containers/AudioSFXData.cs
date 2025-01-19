using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AudioSFXData
{
    public string filePath;
    public string fileName;
    public float volume;
    public float pitch;

    public static List<AudioSFXData> Capture()
    {
        List<AudioSFXData> audioList = new List<AudioSFXData>();

        AudioSource[] sfx = R.AudioSystem.allSFX;

        foreach (var sound in sfx)
        {
            if (!sound.loop)
                continue;

            AudioSFXData data = new AudioSFXData();
            data.volume = sound.volume;
            data.pitch = sound.pitch;
            data.fileName = sound.clip.name;

            string resourcesPath = sound.gameObject.name.Split(AudioSystem.SFX_NAME_FORMAT_CONTAINERS)[1];

            data.filePath = resourcesPath;

            audioList.Add(data);
        }

        return audioList;
    }

    public static void Apply(List<AudioSFXData> sfx)
    {
        AudioSystem audioSystem = R.AudioSystem;
        List<string> cache = new List<string>();

        foreach (var sound in sfx)
        {
            if (!audioSystem.IsPlayingSoundEffect(sound.fileName))
                audioSystem.PlaySoundEffect(sound.filePath, volume: sound.volume, pitch: sound.pitch, loop: true);
            cache.Add(sound.fileName);
        }

        foreach (var source in audioSystem.allSFX)
        {
            if (!cache.Contains(source.clip.name))
                audioSystem.StopSoundEffect(source.clip);
        }
    }
}