using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChannel
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel-[{0}]";

    private List<AudioTrack> tracks = new List<AudioTrack>();

    public int channelIndex { get; private set; }
    public AudioTrack activeTrack { get; private set; }
    public Transform trackContainer { get; private set; } = null;
    private Coroutine co_volumeLeveling = null;

    public AudioChannel(int channel)
    {
        channelIndex = channel;
        trackContainer = new GameObject(string.Format(TRACK_CONTAINER_NAME_FORMAT, channel)).transform;
        trackContainer.SetParent(R.AudioSystem.transform);
    }

    public AudioTrack PlayTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, string filePath)
    {
        if (TryGetTrack(clip.name, out AudioTrack existingTrack))
        {
            if (!existingTrack.IsPlaying)
            {
                existingTrack.Play();
            }

            SetAsActiveTrack(existingTrack);
            return existingTrack;
        }

        AudioTrack track = new AudioTrack(clip, loop, startingVolume, volumeCap, pitch, this, R.AudioSystem.musicMixer,filePath);
        track.Play();
        SetAsActiveTrack(track);
        return track;
    }


    public bool TryGetTrack(string trackName, out AudioTrack value)
    {
        trackName = trackName.ToLower();
        foreach (var track in tracks)
        {
            if (track.name.ToLower() == trackName)
            {
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }

    private void TryStartVolumeLeveling()
    {
        if (co_volumeLeveling.IsNull())
        {
            co_volumeLeveling = R.StartCoroutine(VolumeLeveling());
        }
    }

    private IEnumerator VolumeLeveling()
    {
        while ((activeTrack != null && (tracks.Count > 1 || !Mathf.Approximately(activeTrack.Volume, activeTrack.VolumeCap))) || //Approximately 判断是否相似
               (activeTrack == null && tracks.Count > 0))
        {
            for (int i = tracks.Count - 1; i >= 0; i--)
            {
                AudioTrack track = tracks[i];
                float targetVol = activeTrack == track ? track.VolumeCap : 0;
                if (track == activeTrack && Mathf.Approximately(track.Volume, targetVol))
                    continue;
                track.Volume = Mathf.MoveTowards(track.Volume, targetVol, AudioSystem.TRACK_TRANSITION_SPEED * R.DeltaTime);
                if (track != activeTrack && track.Volume == 0)
                    DestroyTrack(track);
            }

            yield return null;
        }

        co_volumeLeveling = null;
    }

    private void DestroyTrack(AudioTrack track)
    {
        if (tracks.Contains(track))
        {
            tracks.Remove(track);
        }

        Object.Destroy(track.Root);
    }

    private void SetAsActiveTrack(AudioTrack track)
    {
        if (!tracks.Contains(track))
        {
            tracks.Add(track);
        }

        activeTrack = track;
        TryStartVolumeLeveling();
    }

    public void StopTrack(bool immediate = false)
    {
        if (activeTrack == null)
        {
            return;
        }

        if (immediate)
        {
            DestroyTrack(activeTrack);
            activeTrack = null;
        }
        else
        {
            activeTrack = null;
            TryStartVolumeLeveling();
        }
    }
}