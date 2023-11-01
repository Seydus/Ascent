using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance { get; set; }

    #region Built-in Methods
    public void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Saved Data
    public void SaveResolutionData(int resolutionData)
    {
        PlayerPrefs.SetInt("Resolution", resolutionData);
    }

    public void SaveGraphicsData(int graphicsData)
    {
        PlayerPrefs.SetInt("Graphics", graphicsData);
    }

    public void SaveAudioMasterData(float audioMasterData)
    {
        PlayerPrefs.SetFloat("AudioMaster", audioMasterData);
    }

    public void SaveAudioMusicData(float audioMusicData)
    {
        PlayerPrefs.SetFloat("AudioMusic", audioMusicData);
    }

    public void SaveAudioSFXData(float audioSFXData)
    {
        PlayerPrefs.SetFloat("AudioSFX", audioSFXData);
    }

    public void SaveMouseInvertData(float mouseInvertData)
    {
        PlayerPrefs.SetFloat("MouseInvert", mouseInvertData);
    }
    #endregion
}
