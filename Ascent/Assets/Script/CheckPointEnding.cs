using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemCore
{
    public class CheckPointEnding : MonoBehaviour
    {
        [Space]
        [Header("Variables")]
        private bool m_alreadyCollided;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !m_alreadyCollided)
            {
                SceneTransition.Instance.StartCameraCutsceneTransition();
                m_alreadyCollided = true;
            }
        }
    }
}