using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配置
/// </summary>
[System.Serializable]
public class VN_Configuration
{
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.root}vnconfig.cfg";

    public const bool ENCRYPT = false;

    //常规设置
    public bool display_fullscreen = true;
    public string display_resolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReadSpeed = 1f;

    //音频设置
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float voicesVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;
    public bool voicesMute = false;

    //其他设置
    public float historyLogScale = 1f;

    public void Load()
    {
        ConfigMenu.UI_ITEMS ui = ConfigMenu.I.ui;

        // 常规设置
        // 设置窗口大小
        ConfigMenu.I.SetDisplayToFullScreen(display_fullscreen);
        ui.SetButtonColors(ui.fullscreen, ui.windowed, display_fullscreen);

        // 设置窗口分辨率
        int res_index = 0;
        for (int i = 0; i < ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == display_resolution)
            {
                res_index = i;
                break;
            }
        }

        ui.resolutions.value = res_index;

        //设置跳过后继续选项
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        //设置架构和自动阅读速度的值
        ui.architectSpeed.value = dialogueTextSpeed;
        ui.autoReaderSpeed.value = dialogueAutoReadSpeed;

        //设置音频混音器音量
        ui.musicVolume.value = musicVolume;
        ui.sfxVolume.value = sfxVolume;
        ui.voicesVolume.value = voicesVolume;
        ui.musicMute.sprite = musicMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.sfxMute.sprite = sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.voicesMute.sprite = voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;
    }

    public void Save()
    {
        FileSystem.Save(filePath, JsonUtility.ToJson(this), encrypt: ENCRYPT);
    }
}