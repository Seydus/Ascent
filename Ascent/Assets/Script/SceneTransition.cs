using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemCore
{
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; set; }

        #region Components
        public Animator transitionAnim;
        public Animator dialogueAnim;
        public Animator[] textCutsceneAnim;

        public GameObject[] textCutsceneObject;
        public GameObject slider;

        int textInput = 0;
        #endregion

        #region Transition Components
        [Space]
        [Header("Transition")]

        public GameObject cinemachineCamera;

        public CPC_CameraPath cameraPath;
        public GameObject Slider;

        public Transform player;
        public Transform newPlayerPos;
        Vector3 newPos;
        Vector3 rotationVector;

        public bool start;
        #endregion

        #region Built-in Methods
        private void Awake()
        {
            Instance = this;
            newPos = newPlayerPos.position;
            rotationVector = player.rotation.eulerAngles;
            rotationVector.y = 60;
        }

        public void Update()
        {
            if (start)
            {
                StartCameraCutsceneTransition();
                start = false;
            }
        }
        #endregion

        #region Custom Methods
        public void StartCameraCutsceneTransition()
        {
            StartCoroutine(CameraCutsceneTransition());
        }
        #endregion

        #region IEnumerator Data
        IEnumerator CameraCutsceneTransition()
        {
            Debug.Log("CameraCutscene");
            UIController.Instance.isCutscene = true;

            slider.SetActive(false);
            transitionAnim.SetBool("start", false);
            transitionAnim.SetBool("end", true);
            ThirdPersonController.Instance.m_disableMovement = true;
            ThirdPersonController.Instance.anim.SetBool("isWalk", false);

            yield return new WaitForSeconds(1f);

            ThirdPersonController.Instance.isPause = true;
            ThirdPersonController.Instance.mainScaleAddedValue = 2f;
            ThirdPersonController.Instance.colorAddedValue = 0f;
            ThirdPersonController.Instance.vignette_Single.mainColor.a = 0f;
            ThirdPersonController.Instance.vignette_Single.mainScale = 2f;
            
            //Off camera
            //player.position = newPos;
            //player.rotation = Quaternion.Euler(rotationVector);

            cinemachineCamera.SetActive(false);
            Slider.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            cameraPath.enabled = true;

            transitionAnim.SetBool("start", true);
            transitionAnim.SetBool("end", false);

            yield return new WaitForSeconds(9.5f);
            dialogueAnim.SetBool("startDialogue", true);

            yield return new WaitForSeconds(5f);
            dialogueAnim.SetTrigger("endDialogue");
            dialogueAnim.SetBool("startDialogue", false);

            yield return new WaitForSeconds(3f);

            StartCoroutine(TextCutscene());
        }

        IEnumerator TextCutscene()
        {
            //Text 1
            transitionAnim.SetBool("start", true);
            transitionAnim.SetBool("end", false);

            textCutsceneObject[0].SetActive(true);

            yield return new WaitForSeconds(5f);

            textCutsceneAnim[0].SetBool("text_End", true);
            textCutsceneAnim[0].SetBool("text_Idle", false);

            yield return new WaitForSeconds(2f);

            textCutsceneAnim[0].SetBool("text_End", false);
            textCutsceneAnim[0].SetBool("text_Idle", true);

            yield return new WaitForSeconds(0.1f);
            textCutsceneObject[0].SetActive(false);

            //Text 2

            textCutsceneObject[1].SetActive(true);

            yield return new WaitForSeconds(5f);

            textCutsceneAnim[1].SetBool("text_End", true);
            textCutsceneAnim[1].SetBool("text_Idle", false);

            yield return new WaitForSeconds(1f);

            textCutsceneAnim[1].SetBool("text_End", false);
            textCutsceneAnim[1].SetBool("text_Idle", true);

            yield return new WaitForSeconds(0.1f);
            transitionAnim.SetTrigger("ended");
            textCutsceneObject[1].SetActive(false);

            yield return new WaitForSeconds(1.5f);

            SceneManager.LoadScene(0);
            AudioController.Instance.End();
        }
        #endregion
    }
}