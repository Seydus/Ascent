using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace State.Menu
{
    public class MenuController : MonoBehaviour
    {
        public _MenuState[] allMenus;

        public enum MenuState
        {
            Main, Settings, Controls, Credits
        }

        private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

        private _MenuState activeState;

        private Stack<MenuState> stateHistory = new Stack<MenuState>();

        [Space]
        [Header("Main Menu Components")]
        [SerializeField] private Animator transitionAnim;

        [SerializeField] private Slider mouseInvertslider;

        private void Awake()
        {
            ScreenResolutionData(PlayerPrefs.GetInt("Resolution", 5));
            Cursor.lockState = CursorLockMode.Confined;
            mouseInvertslider.value = PlayerPrefs.GetFloat("MouseInvert", 0f);
        }

        private void Start()
        {
            foreach(_MenuState menu in allMenus)
            {
                if(menu == null)
                {
                    continue;
                }

                menu.InitState(menuController: this);

                if(menuDictionary.ContainsKey(menu.state))
                {
                    Debug.LogWarning($"The key <b>{menu.state}</b> already exists in the menu dictionary!");

                    continue;
                }

                menuDictionary.Add(menu.state, menu);
            }

            foreach(MenuState state in menuDictionary.Keys)
            {
                menuDictionary[state].gameObject.SetActive(false);
            }

            SetActiveState(MenuState.Main);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                JumpBack();
            }
        }

        public void JumpBack()
        {
            if(stateHistory.Count <= 1)
            {
                SetActiveState(MenuState.Main);
            }
            else
            {
                stateHistory.Pop();

                SetActiveState(stateHistory.Peek());
            }
        }

        public void SetActiveState(MenuState newState, bool isJumpingBack = false)
        {
            if (!menuDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

                return;
            }

            if (activeState != null)
            {
                activeState.gameObject.SetActive(false);
            }

            activeState = menuDictionary[newState];

            activeState.gameObject.SetActive(true);

            if (!isJumpingBack)
            {
                stateHistory.Push(newState);
            }
        }

        public void PlayGame()
        {
            StartCoroutine(PlayButtonScene());
        }

        public void QuitGame()
        {
            Debug.Log("You quit game!");

            Application.Quit();
        }

        IEnumerator PlayButtonScene()
        {
            transitionAnim.SetTrigger("ended");

            yield return new WaitForSecondsRealtime(1.5f);
            AudioController.Instance.End();

            SceneManager.LoadScene(1);
        }

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

        public void MouseInvertData(float value)
        {
            if (value >= 1)
            {
                DataSaver.Instance.SaveMouseInvertData(value);
            }
            else
            {
                DataSaver.Instance.SaveMouseInvertData(value);
            }
        }
    }
}
