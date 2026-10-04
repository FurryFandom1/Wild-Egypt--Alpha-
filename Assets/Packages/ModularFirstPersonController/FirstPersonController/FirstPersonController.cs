using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController instance;

    private Rigidbody rb;

    // НОВОЕ:
    // Используем настоящий Collider игрока
    // для проверки касания земли.
    private Collider playerCollider;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    private bool isZoomed = false;

    #endregion

    #endregion


    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private bool isWalking = false;


    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;

    public Image sprintBarBG;
    public Image sprintBar;

    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;

    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;

    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion


    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    private bool isGrounded = false;

    #endregion


    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;

    public KeyCode crouchKey = KeyCode.LeftControl;

    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    private PhotonView photonView;

    [HideInInspector]
    public Transform target { private get; set; }

    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion

    #endregion


    #region Head Bob

    public bool enableHeadBob = true;

    public Transform joint;

    public float bobSpeed = 10f;

    public Vector3 bobAmount =
        new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;

    private float timer = 0;

    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // НОВОЕ:
        // Ищем настоящий Collider игрока.
        playerCollider = GetComponent<Collider>();

        if (playerCollider == null)
        {
            playerCollider =
                GetComponentInChildren<Collider>();
        }

        crosshairObject =
            GetComponentInChildren<Image>();

        photonView =
            GetComponent<PhotonView>();


        // Проверяем, является ли объект локальным игроком
        if (photonView.IsMine)
        {
            // Инициализация камеры
            // только для локального игрока
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = fov;
            }
            else
            {
                Debug.LogError(
                    "PlayerCamera is not assigned!"
                );
            }

            originalScale =
                transform.localScale;

            if (joint != null)
            {
                jointOriginalPos =
                    joint.localPosition;
            }

            if (!unlimitedSprint)
            {
                sprintRemaining =
                    sprintDuration;

                sprintCooldownReset =
                    sprintCooldown;
            }
        }
    }


    private void Start()
    {
        if (photonView.IsMine)
        {
            if (lockCursor)
            {
                Cursor.lockState =
                    CursorLockMode.Locked;

                Cursor.visible = false;
            }

            if (crosshair)
            {
                if (crosshairObject != null)
                {
                    crosshairObject.sprite =
                        crosshairImage;

                    crosshairObject.color =
                        crosshairColor;
                }
            }
            else
            {
                if (crosshairObject != null)
                {
                    crosshairObject
                        .gameObject
                        .SetActive(false);
                }
            }

            sprintBarCG =
                GetComponentInChildren<CanvasGroup>();
        }
        else
        {
            if (playerCamera != null)
            {
                playerCamera
                    .gameObject
                    .SetActive(false);
            }

            if (crosshairObject != null)
            {
                crosshairObject
                    .gameObject
                    .SetActive(false);
            }

            this.enabled = false;
        }
    }


    float camRotation;


    private void Update()
    {
        if (!photonView.IsMine)
            return;


        #region Camera

        if (
            playerCamera != null &&
            cameraCanMove
        )
        {
            yaw =
                transform.localEulerAngles.y +
                Input.GetAxis("Mouse X") *
                mouseSensitivity;

            if (!invertCamera)
            {
                pitch -=
                    mouseSensitivity *
                    Input.GetAxis("Mouse Y");
            }
            else
            {
                pitch +=
                    mouseSensitivity *
                    Input.GetAxis("Mouse Y");
            }

            pitch = Mathf.Clamp(
                pitch,
                -maxLookAngle,
                maxLookAngle
            );

            transform.localEulerAngles =
                new Vector3(
                    0,
                    yaw,
                    0
                );

            playerCamera
                .transform
                .localEulerAngles =
                new Vector3(
                    pitch,
                    0,
                    0
                );
        }


        #region Camera Zoom

        if (
            enableZoom &&
            playerCamera != null
        )
        {
            if (
                Input.GetKeyDown(zoomKey) &&
                !holdToZoom &&
                !isSprinting
            )
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }


            if (
                holdToZoom &&
                !isSprinting
            )
            {
                if (
                    Input.GetKeyDown(zoomKey)
                )
                {
                    isZoomed = true;
                }
                else if (
                    Input.GetKeyUp(zoomKey)
                )
                {
                    isZoomed = false;
                }
            }


            if (isZoomed)
            {
                playerCamera.fieldOfView =
                    Mathf.Lerp(
                        playerCamera.fieldOfView,
                        zoomFOV,
                        zoomStepTime *
                        Time.deltaTime
                    );
            }
            else if (
                !isZoomed &&
                !isSprinting
            )
            {
                playerCamera.fieldOfView =
                    Mathf.Lerp(
                        playerCamera.fieldOfView,
                        fov,
                        zoomStepTime *
                        Time.deltaTime
                    );
            }
        }

        #endregion

        #endregion


        #region Sprint

        if (enableSprint)
        {
            if (isSprinting)
            {
                isZoomed = false;

                if (playerCamera != null)
                {
                    playerCamera.fieldOfView =
                        Mathf.Lerp(
                            playerCamera.fieldOfView,
                            sprintFOV,
                            sprintFOVStepTime *
                            Time.deltaTime
                        );
                }

                if (!unlimitedSprint)
                {
                    sprintRemaining -=
                        Time.deltaTime;

                    if (
                        sprintRemaining <= 0
                    )
                    {
                        isSprinting = false;

                        isSprintCooldown =
                            true;
                    }
                }
            }
            else
            {
                sprintRemaining =
                    Mathf.Clamp(
                        sprintRemaining +
                        Time.deltaTime,
                        0,
                        sprintDuration
                    );
            }


            if (isSprintCooldown)
            {
                sprintCooldown -=
                    Time.deltaTime;

                if (
                    sprintCooldown <= 0
                )
                {
                    isSprintCooldown =
                        false;
                }
            }
            else
            {
                sprintCooldown =
                    sprintCooldownReset;
            }


            if (
                useSprintBar &&
                !unlimitedSprint &&
                sprintBar != null
            )
            {
                float sprintRemainingPercent =
                    sprintRemaining /
                    sprintDuration;

                sprintBar
                    .transform
                    .localScale =
                    new Vector3(
                        sprintRemainingPercent,
                        1f,
                        1f
                    );
            }
        }

        #endregion


        // --------------------------------
        // ВАЖНО:
        // Теперь сначала проверяем землю.
        // --------------------------------

        CheckGround();


        #region Jump

        if (
            enableJump &&
            Input.GetKeyDown(jumpKey) &&
            isGrounded
        )
        {
            Jump();
        }

        #endregion


        #region Crouch

        if (enableCrouch)
        {
            if (
                Input.GetKeyDown(crouchKey) &&
                !holdToCrouch
            )
            {
                Crouch();
            }

            if (
                Input.GetKeyDown(crouchKey) &&
                holdToCrouch
            )
            {
                isCrouched = false;

                Crouch();
            }
            else if (
                Input.GetKeyUp(crouchKey) &&
                holdToCrouch
            )
            {
                isCrouched = true;

                Crouch();
            }
        }

        #endregion


        if (
            enableHeadBob &&
            joint != null
        )
        {
            HeadBob();
        }
    }


    private void FixedUpdate()
    {
        #region Movement

        if (!photonView.IsMine)
            return;

        if (playerCanMove)
        {
            Vector3 targetVelocity =
                new Vector3(
                    Input.GetAxis("Horizontal"),
                    0,
                    Input.GetAxis("Vertical")
                );


            if (
                targetVelocity.x != 0 ||
                targetVelocity.z != 0 &&
                isGrounded
            )
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }


            if (
                enableSprint &&
                Input.GetKey(sprintKey) &&
                sprintRemaining > 0f &&
                !isSprintCooldown
            )
            {
                targetVelocity =
                    transform.TransformDirection(
                        targetVelocity
                    ) * sprintSpeed;


                Vector3 velocity =
                    rb.linearVelocity;

                Vector3 velocityChange =
                    targetVelocity -
                    velocity;

                velocityChange.x =
                    Mathf.Clamp(
                        velocityChange.x,
                        -maxVelocityChange,
                        maxVelocityChange
                    );

                velocityChange.z =
                    Mathf.Clamp(
                        velocityChange.z,
                        -maxVelocityChange,
                        maxVelocityChange
                    );

                velocityChange.y = 0;


                if (
                    velocityChange.x != 0 ||
                    velocityChange.z != 0
                )
                {
                    isSprinting = true;

                    if (isCrouched)
                    {
                        Crouch();
                    }

                    if (
                        hideBarWhenFull &&
                        !unlimitedSprint &&
                        sprintBarCG != null
                    )
                    {
                        sprintBarCG.alpha +=
                            5 * Time.deltaTime;
                    }
                }


                rb.AddForce(
                    velocityChange,
                    ForceMode.VelocityChange
                );
            }
            else
            {
                isSprinting = false;


                targetVelocity =
                    transform.TransformDirection(
                        targetVelocity
                    ) * walkSpeed;


                Vector3 velocity =
                    rb.linearVelocity;

                Vector3 velocityChange =
                    targetVelocity -
                    velocity;

                velocityChange.x =
                    Mathf.Clamp(
                        velocityChange.x,
                        -maxVelocityChange,
                        maxVelocityChange
                    );

                velocityChange.z =
                    Mathf.Clamp(
                        velocityChange.z,
                        -maxVelocityChange,
                        maxVelocityChange
                    );

                velocityChange.y = 0;


                rb.AddForce(
                    velocityChange,
                    ForceMode.VelocityChange
                );
            }
        }

        #endregion
    }


    // ========================================
    // НОВАЯ ПРОВЕРКА ЗЕМЛИ
    // ========================================

    private void CheckGround()
    {
        if (playerCollider == null)
        {
            isGrounded = false;
            return;
        }

        Bounds bounds =
            playerCollider.bounds;


        // Точка берётся около нижней части
        // настоящего Collider игрока.
        Vector3 origin =
            new Vector3(
                bounds.center.x,
                bounds.min.y + 0.05f,
                bounds.center.z
            );


        float distance = 0.2f;


        if (
            Physics.Raycast(
                origin,
                Vector3.down,
                out RaycastHit hit,
                distance,
                ~0,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            isGrounded = true;

            Debug.DrawRay(
                origin,
                Vector3.down * distance,
                Color.green
            );
        }
        else
        {
            isGrounded = false;

            Debug.DrawRay(
                origin,
                Vector3.down * distance,
                Color.red
            );
        }
    }


    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(
                0f,
                jumpPower,
                0f,
                ForceMode.Impulse
            );

            isGrounded = false;
        }


        if (
            isCrouched &&
            !holdToCrouch
        )
        {
            Crouch();
        }
    }


    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale =
                new Vector3(
                    originalScale.x,
                    originalScale.y,
                    originalScale.z
                );

            walkSpeed /=
                speedReduction;

            isCrouched = false;
        }
        else
        {
            transform.localScale =
                new Vector3(
                    originalScale.x,
                    crouchHeight,
                    originalScale.z
                );

            walkSpeed *=
                speedReduction;

            isCrouched = true;
        }
    }


    private void HeadBob()
    {
        if (isWalking)
        {
            if (isSprinting)
            {
                timer +=
                    Time.deltaTime *
                    (bobSpeed + sprintSpeed);
            }
            else if (isCrouched)
            {
                timer +=
                    Time.deltaTime *
                    (
                        bobSpeed *
                        speedReduction
                    );
            }
            else
            {
                timer +=
                    Time.deltaTime *
                    bobSpeed;
            }


            joint.localPosition =
                new Vector3(
                    jointOriginalPos.x +
                    Mathf.Sin(timer) *
                    bobAmount.x,

                    jointOriginalPos.y +
                    Mathf.Sin(timer) *
                    bobAmount.y,

                    jointOriginalPos.z +
                    Mathf.Sin(timer) *
                    bobAmount.z
                );
        }
        else
        {
            timer = 0;


            joint.localPosition =
                new Vector3(
                    Mathf.Lerp(
                        joint.localPosition.x,
                        jointOriginalPos.x,
                        Time.deltaTime *
                        bobSpeed
                    ),

                    Mathf.Lerp(
                        joint.localPosition.y,
                        jointOriginalPos.y,
                        Time.deltaTime *
                        bobSpeed
                    ),

                    Mathf.Lerp(
                        joint.localPosition.z,
                        jointOriginalPos.z,
                        Time.deltaTime *
                        bobSpeed
                    )
                );
        }
    }
}


// ========================================================
// CUSTOM EDITOR
// ========================================================

#if UNITY_EDITOR

[CustomEditor(typeof(FirstPersonController))]
[InitializeOnLoadAttribute]

public class FirstPersonControllerEditor : Editor
{
    FirstPersonController fpc;
    SerializedObject SerFPC;


    private void OnEnable()
    {
        fpc =
            (FirstPersonController)target;

        SerFPC =
            new SerializedObject(fpc);
    }


    public override void OnInspectorGUI()
    {
        SerFPC.Update();


        EditorGUILayout.Space();

        GUILayout.Label(
            "Modular First Person Controller",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Bold,

                fontSize = 16
            }
        );


        GUILayout.Label(
            "By Jess Case",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Normal,

                fontSize = 12
            }
        );


        GUILayout.Label(
            "version 1.0.1",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Normal,

                fontSize = 12
            }
        );


        EditorGUILayout.Space();


        #region Camera Setup

        EditorGUILayout.LabelField(
            "",
            GUI.skin.horizontalSlider
        );


        GUILayout.Label(
            "Camera Setup",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Bold,

                fontSize = 13
            },

            GUILayout.ExpandWidth(true)
        );


        EditorGUILayout.Space();


        fpc.playerCamera =
            (Camera)
            EditorGUILayout.ObjectField(
                new GUIContent(
                    "Camera",
                    "Camera attached to the controller."
                ),

                fpc.playerCamera,
                typeof(Camera),
                true
            );


        fpc.fov =
            EditorGUILayout.Slider(
                new GUIContent(
                    "Field of View",
                    "The camera’s view angle."
                ),

                fpc.fov,
                fpc.zoomFOV,
                179f
            );


        fpc.cameraCanMove =
            EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Enable Camera Rotation"
                ),

                fpc.cameraCanMove
            );


        GUI.enabled =
            fpc.cameraCanMove;


        fpc.invertCamera =
            EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Invert Camera Rotation"
                ),

                fpc.invertCamera
            );


        fpc.mouseSensitivity =
            EditorGUILayout.Slider(
                "Look Sensitivity",
                fpc.mouseSensitivity,
                .1f,
                10f
            );


        fpc.maxLookAngle =
            EditorGUILayout.Slider(
                "Max Look Angle",
                fpc.maxLookAngle,
                40f,
                90f
            );


        GUI.enabled = true;


        fpc.lockCursor =
            EditorGUILayout.ToggleLeft(
                "Lock and Hide Cursor",
                fpc.lockCursor
            );


        fpc.crosshair =
            EditorGUILayout.ToggleLeft(
                "Auto Crosshair",
                fpc.crosshair
            );


        if (fpc.crosshair)
        {
            EditorGUI.indentLevel++;


            fpc.crosshairImage =
                (Sprite)
                EditorGUILayout.ObjectField(
                    "Crosshair Image",

                    fpc.crosshairImage,

                    typeof(Sprite),

                    false
                );


            fpc.crosshairColor =
                EditorGUILayout.ColorField(
                    "Crosshair Color",
                    fpc.crosshairColor
                );


            EditorGUI.indentLevel--;
        }


        EditorGUILayout.Space();


        #region Zoom

        GUILayout.Label(
            "Zoom",
            EditorStyles.boldLabel
        );


        fpc.enableZoom =
            EditorGUILayout.ToggleLeft(
                "Enable Zoom",
                fpc.enableZoom
            );


        GUI.enabled =
            fpc.enableZoom;


        fpc.holdToZoom =
            EditorGUILayout.ToggleLeft(
                "Hold to Zoom",
                fpc.holdToZoom
            );


        fpc.zoomKey =
            (KeyCode)
            EditorGUILayout.EnumPopup(
                "Zoom Key",
                fpc.zoomKey
            );


        fpc.zoomFOV =
            EditorGUILayout.Slider(
                "Zoom FOV",

                fpc.zoomFOV,

                .1f,

                fpc.fov
            );


        fpc.zoomStepTime =
            EditorGUILayout.Slider(
                "Step Time",

                fpc.zoomStepTime,

                .1f,

                10f
            );


        GUI.enabled = true;

        #endregion


        #endregion


        #region Movement Setup

        EditorGUILayout.LabelField(
            "",
            GUI.skin.horizontalSlider
        );


        GUILayout.Label(
            "Movement Setup",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Bold,

                fontSize = 13
            },

            GUILayout.ExpandWidth(true)
        );


        EditorGUILayout.Space();


        fpc.playerCanMove =
            EditorGUILayout.ToggleLeft(
                "Enable Player Movement",

                fpc.playerCanMove
            );


        GUI.enabled =
            fpc.playerCanMove;


        fpc.walkSpeed =
            EditorGUILayout.Slider(
                "Walk Speed",

                fpc.walkSpeed,

                .1f,

                fpc.sprintSpeed
            );


        GUI.enabled = true;


        EditorGUILayout.Space();


        #region Sprint

        GUILayout.Label(
            "Sprint",
            EditorStyles.boldLabel
        );


        fpc.enableSprint =
            EditorGUILayout.ToggleLeft(
                "Enable Sprint",

                fpc.enableSprint
            );


        GUI.enabled =
            fpc.enableSprint;


        fpc.unlimitedSprint =
            EditorGUILayout.ToggleLeft(
                "Unlimited Sprint",

                fpc.unlimitedSprint
            );


        fpc.sprintKey =
            (KeyCode)
            EditorGUILayout.EnumPopup(
                "Sprint Key",

                fpc.sprintKey
            );


        fpc.sprintSpeed =
            EditorGUILayout.Slider(
                "Sprint Speed",

                fpc.sprintSpeed,

                fpc.walkSpeed,

                20f
            );


        fpc.sprintDuration =
            EditorGUILayout.Slider(
                "Sprint Duration",

                fpc.sprintDuration,

                1f,

                20f
            );


        fpc.sprintCooldown =
            EditorGUILayout.Slider(
                "Sprint Cooldown",

                fpc.sprintCooldown,

                .1f,

                fpc.sprintDuration
            );


        fpc.sprintFOV =
            EditorGUILayout.Slider(
                "Sprint FOV",

                fpc.sprintFOV,

                fpc.fov,

                179f
            );


        fpc.sprintFOVStepTime =
            EditorGUILayout.Slider(
                "Step Time",

                fpc.sprintFOVStepTime,

                .1f,

                20f
            );


        fpc.useSprintBar =
            EditorGUILayout.ToggleLeft(
                "Use Sprint Bar",

                fpc.useSprintBar
            );


        if (fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;


            fpc.hideBarWhenFull =
                EditorGUILayout.ToggleLeft(
                    "Hide Full Bar",

                    fpc.hideBarWhenFull
                );


            fpc.sprintBarBG =
                (Image)
                EditorGUILayout.ObjectField(
                    "Bar BG",

                    fpc.sprintBarBG,

                    typeof(Image),

                    true
                );


            fpc.sprintBar =
                (Image)
                EditorGUILayout.ObjectField(
                    "Bar",

                    fpc.sprintBar,

                    typeof(Image),

                    true
                );


            fpc.sprintBarWidthPercent =
                EditorGUILayout.Slider(
                    "Bar Width",

                    fpc.sprintBarWidthPercent,

                    .1f,

                    .5f
                );


            fpc.sprintBarHeightPercent =
                EditorGUILayout.Slider(
                    "Bar Height",

                    fpc.sprintBarHeightPercent,

                    .001f,

                    .025f
                );


            EditorGUI.indentLevel--;
        }


        GUI.enabled = true;


        EditorGUILayout.Space();

        #endregion


        #region Jump

        GUILayout.Label(
            "Jump",
            EditorStyles.boldLabel
        );


        fpc.enableJump =
            EditorGUILayout.ToggleLeft(
                "Enable Jump",

                fpc.enableJump
            );


        GUI.enabled =
            fpc.enableJump;


        fpc.jumpKey =
            (KeyCode)
            EditorGUILayout.EnumPopup(
                "Jump Key",

                fpc.jumpKey
            );


        fpc.jumpPower =
            EditorGUILayout.Slider(
                "Jump Power",

                fpc.jumpPower,

                .1f,

                20f
            );


        GUI.enabled = true;


        EditorGUILayout.Space();

        #endregion


        #region Crouch

        GUILayout.Label(
            "Crouch",
            EditorStyles.boldLabel
        );


        fpc.enableCrouch =
            EditorGUILayout.ToggleLeft(
                "Enable Crouch",

                fpc.enableCrouch
            );


        GUI.enabled =
            fpc.enableCrouch;


        fpc.holdToCrouch =
            EditorGUILayout.ToggleLeft(
                "Hold To Crouch",

                fpc.holdToCrouch
            );


        fpc.crouchKey =
            (KeyCode)
            EditorGUILayout.EnumPopup(
                "Crouch Key",

                fpc.crouchKey
            );


        fpc.crouchHeight =
            EditorGUILayout.Slider(
                "Crouch Height",

                fpc.crouchHeight,

                .1f,

                1f
            );


        fpc.speedReduction =
            EditorGUILayout.Slider(
                "Speed Reduction",

                fpc.speedReduction,

                .1f,

                1f
            );


        GUI.enabled = true;


        EditorGUILayout.Space();

        #endregion


        #endregion


        #region Head Bob

        EditorGUILayout.LabelField(
            "",
            GUI.skin.horizontalSlider
        );


        GUILayout.Label(
            "Head Bob Setup",
            new GUIStyle(GUI.skin.label)
            {
                alignment =
                    TextAnchor.MiddleCenter,

                fontStyle =
                    FontStyle.Bold,

                fontSize = 13
            },

            GUILayout.ExpandWidth(true)
        );


        EditorGUILayout.Space();


        fpc.enableHeadBob =
            EditorGUILayout.ToggleLeft(
                "Enable Head Bob",

                fpc.enableHeadBob
            );


        GUI.enabled =
            fpc.enableHeadBob;


        fpc.joint =
            (Transform)
            EditorGUILayout.ObjectField(
                "Camera Joint",

                fpc.joint,

                typeof(Transform),

                true
            );


        fpc.bobSpeed =
            EditorGUILayout.Slider(
                "Speed",

                fpc.bobSpeed,

                1f,

                20f
            );


        fpc.bobAmount =
            EditorGUILayout.Vector3Field(
                "Bob Amount",

                fpc.bobAmount
            );


        GUI.enabled = true;

        #endregion


        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);

            Undo.RecordObject(
                fpc,
                "FPC Change"
            );

            SerFPC
                .ApplyModifiedProperties();
        }
    }
}

#endif