using System;
using UnityEngine;

public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
{
    private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
    private static string[] PARAM_VOLUME = new string[] { "-v", "-vol", "-volume" };
    private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
    private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

    private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel" };
    private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
    private static string[] PARAM_START_VOLUME = new string[] { "-sv", "-startVolume" };
    private static string[] PARAM_SONG = new string[] { "-s", "-song" };
    private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };


    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("PlaySfx", new Action<string[]>(PlaySFx));
        database.AddCommand("StopSfx", new Action<string>(StopSfx));

        database.AddCommand("PlayVoice", new Action<string[]>(PlayVoice));
        database.AddCommand("StopVoice", new Action<string>(StopVoice));


        database.AddCommand("playSong", new Action<string[]>(PlaySong));
        database.AddCommand("playAmbience", new Action<string[]>(PlayAmbience));
        database.AddCommand("StopSong", new Action<string>(StopSong));
        database.AddCommand("StopAmbience", new Action<string>(StopAmbience));
    }

    private static void StopAmbience(string data)
    {
        StopTrack(data.IsNullOrEmpty() ? "0" : data);
    }

    private static void StopSong(string data)
    {
        StopTrack(data.IsNullOrEmpty() ? "1" : data);
    }

    private static void StopTrack(string data)
    {
        if (int.TryParse(data, out int channel))
            R.AudioSystem.StopTrack(channel);
        else
            R.AudioSystem.StopTrack(data);
    }


    private static void PlayAmbience(string[] data)
    {
        string filepath;
        int channel;
        var parameters = ConvertDataToParameters(data);
        //Try to get·the name or path to the track
        parameters.TryGetValue(PARAM_AMBIENCE, out filepath);
        filepath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filepath);
        //Try to get the channel for this track
        parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);
        PlayTrack(filepath, channel, parameters);
    }

    private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
    {
        bool loop;
        bool immediate;
        float volumeCap;
        float startVolume;
        float pitch;
        //Try·to get the max volume of the track
        parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);
        //Try·to get the start volume of the track
        parameters.TryGetValue(PARAM_START_VOLUME, out startVolume, defaultValue: 0f);
        //Try to get the pitch of the track
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
        //Try to get-if this track loops
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

        //Run the 1ogic
        AudioClip sound = R.Load<AudioClip>(filepath);
        if (sound == null)
        {
            Debug.Log($"无法加载语音 ‘{filepath}'");
            return;
        }

        R.AudioSystem.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);
    }

    private static void PlaySong(string[] data)
    {
        string filepath;
        int channel;
        var parameters = ConvertDataToParameters(data);
        //Try to get the·name or path to the track
        parameters.TryGetValue(PARAM_SONG, out filepath);
        filepath = FilePaths.GetPathToResource(FilePaths.resources_music, filepath);
        //Try to get the channel for this track
        parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 1);
        PlayTrack(filepath, channel, parameters);
    }

    private static void StopVoice(string data)
    {
    }

    private static void PlayVoice(string[] data)
    {
        string filepath;
        float volume;
        float pitch;
        bool loop;
        var parameters = ConvertDataToParameters(data);

        //Try to get the name or path to the sound effect
        parameters.TryGetValue(PARAM_SFX, out filepath);
        //Try-to get the volume of·the sound
        parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);
        //Try to get the pitch of the sound
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
        //Try to get if this sound loops
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

        //Run the logic
        AudioClip sound = R.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, filepath));
        if (sound == null)
        {
            $"无法加载语音{filepath}".Log();
            return;
        }

        R.AudioSystem.PlayVoice(sound, volume: volume, pitch: pitch, loop: loop);
    }

    private static void StopSfx(string data)
    {
        R.AudioSystem.StopSoundEffect(data);
    }

    private static void PlaySFx(string[] data)
    {
        string filepath;
        float volume;
        float pitch;
        bool loop;
        var parameters = ConvertDataToParameters(data);

        //Try to get the name or path to the sound effect
        parameters.TryGetValue(PARAM_SFX, out filepath);
        //Try-to get the volume of·the sound
        parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);
        //Try to get the pitch of the sound
        parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
        //Try to get if this sound loops
        parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

        //Run the logic
        AudioClip sound = R.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_sfx, filepath));
        if (sound == null)
            return;

        R.AudioSystem.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);
    }
}