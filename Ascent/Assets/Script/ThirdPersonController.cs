using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour
{
    public static ThirdPersonController Instance { get; set; }

    #region Components
    [SerializeField] private LayerMask deathLayer;
    [SerializeField] private Transform torchLight;
    [SerializeField] private Transform cam;
    [SerializeField] private Light playerLight;
    [SerializeField] private DialogueController dialogueController;
    
    [HideInInspector] public float colorAddedValue;
    [HideInInspector] public float mainScaleAddedValue;
    [HideInInspector] public Q_Vignette_Single vignette_Single;

    [HideInInspector] public Animator anim;
    private CharacterController controller;
    #endregion

    #region Variables
    [Space]
    [Header("Movement")]
    public float walk = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Space]
    [Header("Jump")]
    public Transform groundCheck;
    public LayerMask collidedMask;
    public LayerMask groundMask;
    public LayerMask notGroundMask;

    public float fJumpPressedRememberTime = 0.2f;
    private float fJumpPressedRemember;
    public float jumpHeight;
    public float groundDistance = 0.4f;
    public float gravity = -9.81f;

    bool jumpAgain;
    bool canJump;
    public bool isJumping;
    [SerializeField] private bool isCollidedDown;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNotGrounded;
    [SerializeField] private bool isNotGroundedActivate;

    [Header("Misc")]
    [HideInInspector] public bool m_disableMovement;

    [SerializeField] private Vector3 velocity;

    [Space]
    [Header("Light")]
    [SerializeField] private float size;
    private bool m_checkPoint;
    [HideInInspector] public bool torchEnable = false;

    [Space]
    [Header("Check Point")]
    public Slider frostSlider;
    public Animator transitionAnim;
    public LayerMask checkPointLayer;
    public LayerMask spawnPointLayer;

    [HideInInspector] public bool m_death;
    public int checkNumber;
    public int lastCheckNumber;
    public int spawnCheckNumber;
    public bool isCampfireDialogue;
    public bool isSpawnCampfireDialogue;
    public Animator campfireDialogueAnim;

    private float m_timer;
    private bool m_timerStart;

    public float frostValue;
    public float frostIncreased = 0.05f;
    public float frostDecreased = 0.15f;
    private bool m_fire;

    [Space]
    [Header("Footsteps")]
    FMOD.Studio.EventInstance footStepSound;
    [SerializeField] private Transform audioSource;

    private bool playerIsMoving;

    [Space]
    [Header("Footsteps")]
    public bool isPause;
    #endregion

    #region BuiltIn Methods
    void Awake()
    {
        Instance = this;

        mainScaleAddedValue = 2f;
        frostSlider.value = frostValue;

        controller = GetComponent<CharacterController>();
        footStepSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player_Footsteps");
    }

    void Update()
    {
        Move();
        Jump();

        #region Death Restart Timer
        if (m_timer <= 0)
        {
            anim.SetBool("isRestart", false);
            anim.ResetTrigger("isLand");

            transitionAnim.SetBool("start", true);
            transitionAnim.SetBool("end", false);

            m_timer = 0.5f;
            m_timerStart = false;
            m_disableMovement = false;
        }

        if (m_timerStart)
        {
            m_timer -= Time.deltaTime;
        }
        #endregion

        Frost();
        Death();
        CampfireDialogue();
    }

    public IEnumerator LoadScene()
    {
        transitionAnim.SetBool("start", false);
        transitionAnim.SetBool("end", true);
        anim.SetBool("isWalk", false);
        m_disableMovement = true;

        yield return new WaitForSeconds(1f);

        //Restart to checkpoint
        vignette_Single.mainColor.a = 0f;
        vignette_Single.mainScale = 2f;
        colorAddedValue = 0f;
        mainScaleAddedValue = 2f;

        frostValue = 0f;
        velocity = Vector3.zero;
        Campfire.Instance.CheckPoint();
        transform.position = Campfire.Instance.lastCheckPointPos;

        anim.SetBool("isRestart", true);
        anim.ResetTrigger("isFall");

        m_timerStart = true;

        yield return new WaitForSeconds(0.5f);

        transitionAnim.SetBool("start", true);
        transitionAnim.SetBool("end", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, size);
    }
    #endregion

    #region Custom Methods
    public void Move()
    {
        if (!m_disableMovement)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                controller.Move(moveDir.normalized * walk * Time.deltaTime);

                anim.SetBool("isWalk", true);
                anim.SetBool("isIdle", false);
            }
            else
            {
                anim.SetBool("isWalk", false);
                anim.SetBool("isIdle", true);
            }

            if (isGrounded && direction.magnitude >= 0.01f || direction.magnitude <= -0.01f)
            {
                playerIsMoving = true;
            }
            else if (direction.magnitude == 0f)
            {
                playerIsMoving = false;
            }
        }
    }

    public void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isNotGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, notGroundMask);

        if (isGrounded && velocity.y < 0)
        {
            anim.SetBool("isNotFall", true);
            velocity.y = -2f;

            if (isJumping)
            {
                anim.SetTrigger("isLand");
                anim.ResetTrigger("isFall");
                isJumping = false;
                canJump = false;
            }
            else
            {
                anim.ResetTrigger("isLand");
            }
        }

        if (isNotGrounded && velocity.y < 0)
        {
            anim.SetBool("isNotFall", true);
            velocity.y = -2f;

            if (isJumping)
            {
                anim.SetTrigger("isLand");
                anim.ResetTrigger("isFall");
                isJumping = false;
                canJump = false;
            }
            else
            {
                anim.ResetTrigger("isLand");
            }
        }

        isCollidedDown = Physics.CheckSphere(groundCheck.position, groundDistance, collidedMask);

        if (!isCollidedDown && !isJumping && !canJump)
        {
            anim.SetTrigger("isFall");
            anim.SetBool("isNotFall", false);
        }

        if (Input.GetButtonDown("Jump") && !m_disableMovement)
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }
        else
        {
            fJumpPressedRemember -= Time.deltaTime;
        }

        if ((fJumpPressedRemember > 0) && isGrounded && !isJumping && !canJump)
        {
            fJumpPressedRemember = 0;
            anim.SetTrigger("isJump");
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Death()
    {
        m_death = Physics.CheckSphere(groundCheck.position, size, deathLayer);

        if (m_death)
        {
            StartCoroutine(LoadScene());
        }
    }

    public void Frost()
    {
        if (isPause)
            return;

        m_fire = Physics.CheckSphere(groundCheck.position, size, checkPointLayer);

        if (m_fire)
        {
            if (frostValue > 0)
            {
                if (checkNumber <= lastCheckNumber)
                {
                    frostValue -= frostDecreased * Time.deltaTime;

                    if (!isPause)
                    {
                        colorAddedValue -= 0.12f * Time.deltaTime;
                        vignette_Single.mainColor.a = colorAddedValue;

                        if (colorAddedValue <= 0f)
                        {
                            colorAddedValue = 0;
                        }

                        mainScaleAddedValue -= 1f * Time.deltaTime;
                        vignette_Single.mainScale = mainScaleAddedValue;

                        if (mainScaleAddedValue <= 2f)
                        {
                            mainScaleAddedValue = 2f;
                        }
                    }
                }
                else
                {
                    if (frostValue < 1.5)
                    {
                        frostValue += frostIncreased * Time.deltaTime;
                    }

                    if (frostValue > 1)
                    {
                        StartCoroutine(LoadScene());
                    }
                }
            }
        }
        else
        {
            if (frostValue < 1.5)
            {
                frostValue += frostIncreased * Time.deltaTime;
            }

            if (frostValue > 1)
            {
                StartCoroutine(LoadScene());
            }
            if (frostValue >= 0.15f)
            {
                colorAddedValue += 0.04f * Time.deltaTime;
                vignette_Single.mainColor.a = colorAddedValue;

                if (colorAddedValue >= 0.3f)
                {
                    colorAddedValue = 0.3f;
                }
            }

            if (frostValue >= 0.4f)
            {
                mainScaleAddedValue += 0.4f * Time.deltaTime;
                vignette_Single.mainScale = mainScaleAddedValue;

                if (mainScaleAddedValue >= 4f)
                {
                    mainScaleAddedValue = 4f;
                }
            }
        }
        frostSlider.value = frostValue;
    }

    public void ApplyJumpForce()
    {
        //Add a timestamp on the animation setings
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        canJump = true;
        footStepSound.start();
    }

    public void CampfireDialogue()
    {
        isCampfireDialogue = Physics.CheckSphere(groundCheck.position, groundDistance, checkPointLayer);

        isSpawnCampfireDialogue = Physics.CheckSphere(groundCheck.position, groundDistance, spawnPointLayer);

        if (isCampfireDialogue)
        {
            if (checkNumber == lastCheckNumber)
            {
                OpenDialogue();
                lastCheckNumber++;
            }

            if (checkNumber <= lastCheckNumber)
            {
                spawnCheckNumber = checkNumber;
            }
        }

        if (isSpawnCampfireDialogue)
        {
            if (checkNumber == lastCheckNumber)
            {
                lastCheckNumber++;
            }
        }
    }

    public void OpenDialogue()
    {
        StartCoroutine(StartCampfireDialogue());
    }

    public IEnumerator StartCampfireDialogue()
    {
        campfireDialogueAnim.SetBool("intro", true);
        campfireDialogueAnim.SetBool("end", false);
        anim.SetBool("isWalk", false);

        m_disableMovement = true;

        yield return new WaitForSeconds(1.3f);

        dialogueController.NextSentence();
        dialogueController.m_nextText = false;
        dialogueController.m_interacted = true;

        yield return new WaitForSeconds(7f);

        m_disableMovement = false;

        campfireDialogueAnim.SetBool("intro", false);
        campfireDialogueAnim.SetBool("end", true);
    }

    public void Landing()
    {
        isJumping = true;
    }

    public void Footsteps()
    {
        if (playerIsMoving)
        {
            footStepSound.start();
        }
    }
    #endregion
}