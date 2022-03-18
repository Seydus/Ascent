using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Cinemachine;

namespace SystemCore
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; set; }

        #region UI Components
        [Space]
        [Header("UI Components")]
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private GameObject main;
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject frozeSlider;

        [SerializeField] private Slider mouseInvertslider;

        [SerializeField] private DataSaver data;

        [Space]
        [SerializeField] private Animator transitionAnim;

        public CinemachineFreeLook vcam;

        [Space]
        [Header("Variables")]
        private bool m_gameMenuOpen;
        public bool isCutscene;

        public float mouxeYInvertValue;
        #endregion

        #region Built-in Methods
        private void Awake()
        {
            Instance = this;

            GraphicsData(PlayerPrefs.GetInt("Graphics", 3));

            mouxeYInvertValue = PlayerPrefs.GetFloat("MouseInvert", 0f);

            mouseInvertslider.value = PlayerPrefs.GetFloat("MouseInvert", 0f);

            if (mouxeYInvertValue >= 1f)
            {
                vcam.m_YAxis.m_InvertInput = false;
            }
            else
            {
                vcam.m_YAxis.m_InvertInput = true;
            }
        }

        private void Start()
        {
            m_gameMenuOpen = true;
        }

        void Update()
        {
            if (!isCutscene)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (m_gameMenuOpen)
                    {
                        OpenGameMenu();
                    }
                    else
                    {
                        CloseGameMenu();
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
        }

        IEnumerator LoadMenuScene()
        {
            transitionAnim.SetTrigger("ended");
            AudioController.Instance.End();

            yield return new WaitForSecondsRealtime(1.5f);

            SceneManager.LoadScene(0);
            Time.timeScale = 1;       
        }
        #endregion

        #region Custom Methods
        private void OpenGameMenu()
        {
            gameMenu.SetActive(true);
            frozeSlider.SetActive(false);

            m_gameMenuOpen = !m_gameMenuOpen;

            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            ThirdPersonController.Instance.isPause = true;
        }

        private void CloseGameMenu()
        {
            gameMenu.SetActive(false);

            frozeSlider.SetActive(true);

            main.SetActive(true);
            settings.SetActive(false);

            m_gameMenuOpen = !m_gameMenuOpen;

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            ThirdPersonController.Instance.isPause = false;
        }
        #endregion

        #region  Game Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            switch (buttonType)
            {
                case "Resume":
                    CloseGameMenu();
                    break;
                case "Restart":
                    CloseGameMenu();
                    ThirdPersonController.Instance.StartCoroutine(ThirdPersonController.Instance.LoadScene());
                    break;
                case "Settings":
                    main.SetActive(false);
                    settings.SetActive(true);
                    break;
                case "Main Menu":
                    StartCoroutine(LoadMenuScene());
                    break;
                case "Back":
                    main.SetActive(true);
                    settings.SetActive(false);
                    break;
            }
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

            data.SaveGraphicsData(digit);
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

            data.SaveResolutionData(digit);
        }
        #endregion

        #region Mouse Invert
        public void MouseInvertData(float value)
        {
            if (value >= 1)
            {
                vcam.m_YAxis.m_InvertInput = false;
                DataSaver.Instance.SaveMouseInvertData(value);
            }
            else
            {
                vcam.m_YAxis.m_InvertInput = true;
                DataSaver.Instance.SaveMouseInvertData(value);
            }
        }
        #endregion
    }
}
