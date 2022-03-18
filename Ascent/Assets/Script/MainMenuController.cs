using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace UISystem
{
    public class MainMenuController : MonoBehaviour
    {
        #region Main Menu Components
        [Space]
        [Header("Main Menu Components")]
        [SerializeField] private GameObject main;
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject controls;
        [SerializeField] private GameObject credits;
        [SerializeField] private Animator transitionAnim;

        [SerializeField] private Slider mouseInvertslider;

        #endregion

        #region Built-in Methods
        private void Awake()
        {
            ScreenResolutionData(PlayerPrefs.GetInt("Resolution", 5));
            Cursor.lockState = CursorLockMode.Confined;
            mouseInvertslider.value = PlayerPrefs.GetFloat("MouseInvert", 0f);
        }

        #region IEnumerator
        IEnumerator PlayButtonScene()
        {
            transitionAnim.SetTrigger("ended");

            yield return new WaitForSecondsRealtime(1.5f);
            AudioController.Instance.End();

            SceneManager.LoadScene(1);
        }
        #endregion

        #endregion   

        #region  Game Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            switch (buttonType)
            {
                case "Play":
                    StartCoroutine(PlayButtonScene());
                    break;
                case "Settings":
                    main.SetActive(false);
                    settings.SetActive(true);
                    break;
                case "Controls":
                    main.SetActive(false);
                    controls.SetActive(true);
                    break;
                case "Credits":
                    main.SetActive(false);
                    credits.SetActive(true);
                    break;
                case "Exit":
                    Application.Quit();
                    break;
                case "Back":
                    main.SetActive(true);
                    settings.SetActive(false);
                    controls.SetActive(false);
                    credits.SetActive(false);
                    break;
            }
        }
        #endregion

        #region ResolutionData
        public void ScreenResolutionData(int digit)
        {
            switch (digit)
            {
                case 1:
                    Screen.SetResolution(640, 480, true);
                    break;
                case 2:
                    Screen.SetResolution(800, 600, true);
                    break;
                case 3:
                    Screen.SetResolution(1024, 576, true);
                    break;
                case 4:
                    Screen.SetResolution(1440, 900, true);
                    break;
                case 5:
                    Screen.SetResolution(1366, 768, true);
                    break;
                case 6:
                    Screen.SetResolution(1920, 1080, true);
                    break;
            }

            DataSaver.Instance.SaveResolutionData(digit);
        }
        #endregion

        #region Graphics Data
        public void GraphicsData(int digit)
        {
            switch (digit)
            {
                case 1:
                    QualitySettings.SetQualityLevel(0);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(1);
                    break;
                case 3:
                    QualitySettings.SetQualityLevel(2);
                    break;
            }

            DataSaver.Instance.SaveGraphicsData(digit);
        }
        #endregion 
        
        #region Mouse Invert
        public void MouseInvertData(float value)
        {
            if(value >= 1)
            {
                DataSaver.Instance.SaveMouseInvertData(value);
            }
            else
            {
                DataSaver.Instance.SaveMouseInvertData(value);
            }
        }
        #endregion
    }
}
