using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; set; }
    public DataSaver dataSaver;

    #region FMOD
    [Space]
    [Header("FMOD")]
    [HideInInspector] public FMOD.Studio.EventInstance windSound;
    [HideInInspector] public FMOD.Studio.EventInstance ambienceSound;

    FMOD.Studio.Bus master;
    FMOD.Studio.Bus music;
    FMOD.Studio.Bus sfx;
    #endregion

    #region Components
    [Space]
    [Header("Components")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    #endregion

    #region Variables
    [Space]
    [Header("Variables")]
    private float masterVolume = 1f;
    private float musicVolume = 0.5f;
    private float sfxVolume = 0.5f;
    #endregion

    #region Built-in Methods
    private void Awake()
    {
        Instance = this;

        #region Referenced
        master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        sfx = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");

        windSound = FMODUnity.RuntimeManager.CreateInstance("event:/Music/wind");
        ambienceSound = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Ambient");

        #endregion

        #region Call out Saved Data
        MasterVolumeLevel(PlayerPrefs.GetFloat("AudioMaster", 1f));
        MusicVolumeLevel(PlayerPrefs.GetFloat("AudioMusic", 0.5f));
        SFXVolumeLevel(PlayerPrefs.GetFloat("AudioSFX", 0.5f));

        masterSlider.value = PlayerPrefs.GetFloat("AudioMaster", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("AudioMusic", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("AudioSFX", 0.5f);
        #endregion

        windSound.start();
        ambienceSound.start();
    }

    private void Update()
    {
        master.setVolume(masterVolume);
        music.setVolume(musicVolume);
        sfx.setVolume(sfxVolume);
    }
    #endregion

    #region Custom Methods
    public void MasterVolumeLevel(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
        dataSaver.SaveAudioMasterData(masterVolume);
    }

    public void MusicVolumeLevel(float newMusicVolume)
    {
        musicVolume = newMusicVolume;
        dataSaver.SaveAudioMusicData(musicVolume);
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        sfxVolume = newSFXVolume;
        dataSaver.SaveAudioSFXData(sfxVolume);
    }

    public void End()
    {
        master.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    #endregion
}
