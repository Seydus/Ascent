using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Introduction : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim;

    public DataSaver dataSaver;

    #region FMOD
    [Space]
    [Header("FMOD")]
    [HideInInspector] public FMOD.Studio.EventInstance windSound;

    FMOD.Studio.Bus master;
    FMOD.Studio.Bus sfx;
    #endregion

    #region Components
    [Space]
    [Header("Components")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;

    #endregion

    #region Variables
    [Space]
    [Header("Variables")]
    private float masterVolume = 1f;
    private float sfxVolume = 0.5f;
    #endregion

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;


        #region Referenced
        master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        sfx = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");

        windSound = FMODUnity.RuntimeManager.CreateInstance("event:/Music/wind");
        #endregion

        #region Call out Saved Data
        MasterVolumeLevel(PlayerPrefs.GetFloat("AudioMaster", 1f));
        SFXVolumeLevel(PlayerPrefs.GetFloat("AudioSFX", 0.5f));

        masterSlider.value = PlayerPrefs.GetFloat("AudioMaster", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("AudioSFX", 0.5f);
        #endregion

        windSound.start();
    }

    public void ClickToStartGame()
    {
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        master.setVolume(masterVolume);
        sfx.setVolume(sfxVolume);
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
        dataSaver.SaveAudioMasterData(masterVolume);
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

    IEnumerator StartGame()
    {
        //AudioController.Instance.End();
        SceneManager.LoadScene(2);
        End();
        yield return new WaitForSecondsRealtime(0f);
    }
}
