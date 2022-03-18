using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{

    #region Variables
    [TextArea, Header("Dialogue")]
    [Space]
    public string[] sentences;

    private int m_index = 0;

    public float dialogueSpeed;

    public bool m_nextText = true;
    [HideInInspector] public bool m_interacted;

    private bool stopDialogue;

    #endregion

    #region Components
    public TextMeshProUGUI dialogueText;
    #endregion

    private void Awake()
    {
        m_nextText = true;
    }

    private void LateUpdate()
    {
        dialogueText.text = sentences[m_index];

        if (m_interacted)
        {
            if (m_nextText)
            {
                NextSentence();

                m_nextText = false;
                m_interacted = false;
            }
        }
    }

    public void NextSentence()
    {
        if (m_index <= sentences.Length - 1)
        {
            dialogueText.text = "";
            StartCoroutine(WriteSentence());
        }
        else
        {
            StopDialogue();
        }
    }

    IEnumerator WriteSentence()
    {
        yield return new WaitForSeconds(0.5f);
        m_nextText = true;
        m_interacted = false;
    }

    public void EndDialogue()
    {
        m_index++;
    }

    public void StopDialogue()
    {
        m_index = 0;
        m_nextText = true;

        m_index++;
        stopDialogue = true;
    }

    public void StartDialogue()
    {
        stopDialogue = false;
    }
}
