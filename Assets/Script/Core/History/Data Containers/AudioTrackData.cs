using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音轨数据
/// </summary>
[System.Serializable]
public class AudioTrackData
{
    public int channel = 0;
    public string trackName;
    public string trackPath;
    public float trackVolume;
    public float trackPitch;
    public bool loop;

    public AudioTrackData(AudioChannel channel)
    {
        this.channel = channel.channelIndex;

        if (channel.activeTrack == null)
            return;

        var track = channel.activeTrack;
        trackName = track.name;
        trackPath = track.path;
        trackVolume = track.VolumeCap;
        trackPitch = track.pitch;
        loop = track.Loop;
    }

    public static List<AudioTrackData> Capture()
    {
        List<AudioTrackData> audioChannels = new List<AudioTrackData>();

        foreach (KeyValuePair<int, AudioChannel> channel in R.AudioSystem.channels)
        {
            if (channel.Value.activeTrack == null)
                continue;

            AudioTrackData data = new AudioTrackData(channel.Value);
            audioChannels.Add(data);
        }

        return audioChannels;
    }

    public static void Apply(List<AudioTrackData> data)
    {
        List<int> cache = new List<int>();

        foreach (var channelData in data)
        {
            AudioChannel channel = R.AudioSystem.TryGetChannel(channelData.channel, createIfDoesNotExist: true);
            if (channel.activeTrack == null || channel.activeTrack.name != channelData.trackName)
            {
                AudioClip clip = HistoryCache.LoadAudio(channelData.trackPath);
                if (clip != null)
                {
                    channel.StopTrack(immediate: true);
                    channel.PlayTrack(clip, channelData.loop, channelData.trackVolume, channelData.trackVolume, channelData.trackPitch, channelData.trackPath);
                }
                else
                    Debug.LogWarning($"历史记录状态：无法加载音轨 '{channelData.trackPath}'");
            }

            cache.Add(channelData.channel);
        }

        foreach (KeyValuePair<int, AudioChannel> channel in R.AudioSystem.channels)
        {
            if (!cache.Contains(channel.Value.channelIndex))
                channel.Value.StopTrack(immediate: true);
        }
    }
}