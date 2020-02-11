using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using DG.Tweening;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Player           player;
    public static HUD              hud;
    public static Menu             menu;
    public static CameraTransition cameraTransition;

    public static WaterRippleForScreens.RippleEffect rippleEffect;

    private LevelExit levelExit;

    [SerializeField] private HUD              currentHud;
    [SerializeField] private Menu             currentMenu;
    [SerializeField] private CameraTransition currentCameraTransition;

    private EnemyCounter _enemyCounter;

    private FourMovingPlatforms fourMovingPlatforms;

    public static bool AllowUseOfPortals, IsMenuDisabled, IsBossAnimationPlaying;
    public static bool AreControlsEnabled = true;

    public static Camera                       mainCamera;
    public static CinemachineStateDrivenCamera stateDrivenCamera;
    public static CinemachineFramingTransposer runVirtualCamera;

    public static bool IsPanelOpen,
                       IsTransitioning,
                       IsDungeonPanelEntryOpen,
                       IsInfoPanelOpen,
                       IsAbilityPanelOpen;

    [Header("Recall Timers")]
    [SerializeField]
    private float recallKeyPressHoldDuration = .5f;

    private bool  isRecallKeyPressed;
    private float uiDoorOpenWaitTime = .4f;

    [Header("HitStop")]
    [SerializeField]
    private float hitStopDuration = .5f;

    private bool waiting;
    public bool peekDisabled;

    [Header("Cheats")]
    public bool enableCheats = false;

    public GameObject cheatEnabledText;

    private void OnEnable () { SceneManager.sceneLoaded += OnSceneLoaded; }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        mainCamera        = GameObject.Find("Main Camera").GetComponent<Camera>();
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        runVirtualCamera  = FindObjectOfType<CameraShaker>().GetComponent<CinemachineFramingTransposer>();

        player            = FindObjectOfType<Player>();
        rippleEffect      = FindObjectOfType<WaterRippleForScreens.RippleEffect>();
        cameraTransition  = currentCameraTransition;
        AllowUseOfPortals = false;
        menu              = currentMenu;
        hud               = currentHud;

        PostProcessingController.Instance.SetVolumeObject(mainCamera.GetComponent<PostProcessVolume>());

        if (scene.buildIndex <= 3) { PostProcessingController.Instance.SetToZeroIntensity(); }
        else { PostProcessingController.Instance.SetToDungeonIntensity(); }
    }

    private void Awake ()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mainCamera        = GameObject.Find("Main Camera").GetComponent<Camera>();
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        runVirtualCamera  = FindObjectOfType<CameraShaker>().GetComponent<CinemachineFramingTransposer>();

        player       = FindObjectOfType<Player>();
        rippleEffect = FindObjectOfType<WaterRippleForScreens.RippleEffect>();
        hud          = currentHud;
        menu         = currentMenu;
    }

    private void Start () { cameraTransition = FindObjectOfType<CameraTransition>(); }

    void Update ()
    {
        cheatEnabledText.SetActive(enableCheats);
        
        if (enableCheats)
            RespondToDebugKeys();
        
        if (IsBossAnimationPlaying == false)
        {
            RespondToAbilityKey();
            RespondToRecallKey();

            if (IsTransitioning == false)
            {
                RespondToCameraPeekKey();
                RespondToMenuKey();
                RespondToCloseKey();
            }
        }

        hud.UpdateUpgradeImage();

        ResetRecallDuration();
    }

    private void RespondToDebugKeys ()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == SceneManager.sceneCountInBuildSettings - 1) { return; }

            if (SceneManager.GetActiveScene().name == "Dungeon II-II") Physics2D.IgnoreLayerCollision(12, 13, false);

            SceneManager.LoadScene(sceneIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerStats.ResetHealth();
            PlayerStats.ResetStamina();
            HealthBar.ResetHealthBarStatus();
            StaminaBar.ResetStaminaBarStatus();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerStats.Coins += 30;
            EssentialObjects.UpdateCoinsStatic();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerStats.Coins -= 30;
            EssentialObjects.UpdateCoinsStatic();
        }

        if (Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene("Escape");
    }

    private void RespondToAbilityKey ()
    {
        if (InputControl.GetButtonDown("Ability Panel"))
        {
            if (player.isDeactivated)
            {
                return;
            }

            if (menu.gameObject.activeSelf)
            {
                return;
            }

            //if (menu.canBeClosed == false) { return; }

            if (IsPanelOpen)
            {
                return;
            }

            if (menu.isAnimating)
            {
                return;
            }

            //if (IsMenuDisabled) { return; }

//            if (IsDialogueActive)
//            {
//                Debug.Log("Dialog is active");
//                return;
//            }

            if (hud.DialoguePanel.activeSelf) { return; }

            //if (IsInfoPanelOpen) { return; }

            if (hud.abilityPanel.activeSelf)
            {
                //IsAbilityPanelOpen = false;
                hud.abilityPanel.gameObject.SetActive(false);
            }
            else
            {
                //IsAbilityPanelOpen = true;
                hud.abilityPanel.gameObject.SetActive(true);
            }
        }
    }

    private void RespondToCloseKey ()
    {
        if (InputControl.GetButtonDown("Close"))
        {
            if (hud.abilityPanel.activeSelf)
            {
                //IsAbilityPanelOpen = false;
                hud.abilityPanel.gameObject.SetActive(false);
            }

            if (menu.gameObject.activeSelf && menu.canBeClosed && menu.isAnimating == false) menu.CloseMenu();

            if (menu.isApprovePanelOpen) menu.CloseApprovePanel();

            if (IsDungeonPanelEntryOpen) levelExit.CloseDungeonPanel();
        }
    }

    private void RespondToRecallKey ()
    {
        if (hud.abilityPanel.activeSelf) { return; }

        if (SceneManager.GetActiveScene().buildIndex > 4 && SceneManager.GetActiveScene().buildIndex < 21)
        {
            if (InputControl.GetButton("Return To Base") && IsTransitioning == false &&
                player.isDeactivated                                                     == false)
            {
                if (!SceneManager.GetActiveScene().name.Equals("Tutorial"))
                {
                    if (recallKeyPressHoldDuration > 0f)
                    {
                        recallKeyPressHoldDuration -= Time.deltaTime;
                        hud.UpdateDoorFrame(recallKeyPressHoldDuration);
                        isRecallKeyPressed = true;
                    }
                    else
                        RecallToVillage();
                }
            }
        }
    }

    private void ResetRecallDuration ()
    {
        if (InputControl.GetButtonUp("Return To Base") && isRecallKeyPressed) isRecallKeyPressed = false;

        if (isRecallKeyPressed == false && recallKeyPressHoldDuration < 1f && hud.IsDoorOpen() == false)
        {
            recallKeyPressHoldDuration += Time.deltaTime;

            if (recallKeyPressHoldDuration > 1f) recallKeyPressHoldDuration = 1f;

            hud.UpdateDoorFrame(recallKeyPressHoldDuration);
        }
    }

    private void RespondToCameraPeekKey ()
    {
        if (player.GetIsJumping() || player.GetIsRollingMidAir() || peekDisabled || menu.gameObject.activeSelf || hud.abilityPanel.activeSelf) { return; }
        
        if (InputControl.GetButton("Peek"))
            CinemachineOffset.Instance.OffsetCamera();
        else
            CinemachineOffset.Instance.ResetCamera();

        if (InputControl.GetButtonUp("Peek")) CinemachineOffset.Instance.ResetCamera();
    }

    private void RespondToMenuKey ()
    {
        if (player.isDeactivated) { return; }

        if (menu.canBeClosed == false) { return; }

        if (IsPanelOpen) { return; }

        if (menu.isAnimating) { return; }

        if (IsMenuDisabled) { return; }

        //if (IsDialogueActive) { return; }
        if (hud.DialoguePanel.activeSelf) { return; }

        if (IsInfoPanelOpen) { return; }

        if (InputControl.GetButtonDown("Menu"))
        {
            if (IsDungeonPanelEntryOpen)
            {
                levelExit.CloseDungeonPanel();
                return;
            }

            if (hud.abilityPanel.activeSelf)
            {
                hud.abilityPanel.gameObject.SetActive(false);
                player.isDeactivated = false;
                //IsAbilityPanelOpen   = false;
                return;
            }

            if (menu.isApprovePanelOpen)
            {
                menu.CloseApprovePanel();
                return;
            }

            if (menu.gameObject.activeSelf)
                menu.CloseMenu();
            else
                menu.OpenMenu();
        }
    }

    public void SendLevelExitObj (LevelExit exit) { levelExit = exit; }

    public void ReturnToCheckpoint ()
    {
        cameraTransition.StartSwipeIn();

        Invoke(nameof(ReloadScene), 1.4f);
    }

    public void ReloadHouse ()
    {
        cameraTransition.StartSwipeIn();

        Invoke(nameof(InitializeReloadHouse), 1.4f);
    }

    private void ReturnToVillage ()
    {
        player.GetComponent<Animator>().SetTrigger("isTeleporting");
        LevelManager.LoadVillageScene();
    }

    private void RecallToVillage () // if pressed b key
    {
        player.GetComponent<Animator>().SetTrigger("recall");
        player.isDeactivated = true;

        if (SceneManager.GetActiveScene().buildIndex >= 6 && SceneManager.GetActiveScene().buildIndex <= 11)
            DungeonRecall.FirstDungeonRecallSceneName = SceneManager.GetActiveScene().name;
        else if (SceneManager.GetActiveScene().buildIndex >= 13 && SceneManager.GetActiveScene().buildIndex <= 16)
            DungeonRecall.SecondDungeonRecallSceneName = SceneManager.GetActiveScene().name;

        Invoke(nameof(SwipeIn),          uiDoorOpenWaitTime);
        Invoke(nameof(InitializeRecall), 1.4f + uiDoorOpenWaitTime);
    }

    private void SwipeIn () { cameraTransition.StartSwipeIn(); }

    private void InitializeRecall ()
    {
        hud.UpdateDoorAnimation();
        LevelManager.LoadVillageScene();
    }

    private void ReloadScene ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        player.GetComponent<CapsuleCollider2D>().sharedMaterial = player.zeroFriction;
        player.GetComponent<Rigidbody2D>().bodyType             = RigidbodyType2D.Dynamic;
        player.gameObject.transform.position                    = PlayerStats.CheckpointLocation;

        player.GetComponent<Animator>().SetTrigger("isReset");
        player.IsAlive(true);

        Physics2D.IgnoreLayerCollision(player.collisionLayer,
                                       player.playerLayer, false);

        PlayerStats.ResetHealth();
        PlayerStats.ResetStamina();
        HealthBar.ResetHealthBarStatus();
        StaminaBar.ResetStaminaBarStatus();

        cameraTransition.StartSwipeIn();
    }

    private void InitializeReloadHouse ()
    {
        SceneManager.LoadScene("Rand's House");

        PlayerStats.ResetHealth();
        PlayerStats.ResetStamina();
        HealthBar.ResetHealthBarStatus();
        StaminaBar.ResetStaminaBarStatus();

        cameraTransition.StartSwipeIn();
    }

    public void LoadEscapeLevel () { StartCoroutine(InitializeLoadEscapeLevel()); }

    private IEnumerator InitializeLoadEscapeLevel ()
    {
        yield return new WaitForSeconds(3f);

        cameraTransition.StartSwipeIn();

        yield return new WaitForSeconds(1.4f);

        SceneManager.LoadScene("Escape");
    }

    public void HitStopEffect ()
    {
        if (waiting) return;

        Time.timeScale = .01f;
        StartCoroutine(HitStop(hitStopDuration));
    }

    private IEnumerator HitStop (float hitStopDuration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
        waiting        = false;
    }
    
    [ DllImport("__Internal") ]
    private static extern void ReplayEvent (int level);
}