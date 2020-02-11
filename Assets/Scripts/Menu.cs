using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Menu : MonoBehaviour
{
    [Header("Continue Button")]
    [SerializeField]
    private GameObject newGameButton;

    [SerializeField] private Button continueButton;

    [Header("Option Buttons")]
    [SerializeField]
    private GameObject musicDisabledImage;

    [SerializeField] private GameObject soundDisabledImage;

    [Header("Menu Panel")]
    [SerializeField]
    private GameObject menuPanel;

    [Header("Approve Panel")]
    [SerializeField]
    private GameObject approvePanel;

    [SerializeField] private Button disapproveButton;

    private Animator        animator;
    private AnimationClip[] clips;

    private float delayTime;
    public  bool  isAnimating;
    private bool  isFirstPlay, canBeClicked;
    public  bool  canBeClosed;
    public  bool  isApprovePanelOpen;

    [SerializeField] private Button[] buttons;

    [Header("Controls Panel")]
    [SerializeField]
    private GameObject controlsPanel;

    [SerializeField] private Button[] controlButtons;

    [Header("Assist Panel Code")]
    [SerializeField]
    private GameObject assistPanel;

    [SerializeField] private Button[] assistButtons;

    private int  newEnemyStrength,      newHazardDamage;
    private bool isInfiniteDashChanged, isInvicibleChanged;

    public TextMeshProUGUI enemyStrengthText, hazardDamageText, staminaDrainText, infiniteDashText, invicibleText;

    private void Awake ()
    {
        //DisableButtonNavigation();

        animator = GetComponent<Animator>();
    }

    private void Start ()
    {
        if (canBeClosed == false)
            OpenMenu();
        else
            CloseMenu();

        GameManager.player.isDeactivated = true;
    }

    private void OnEnable ()
    {
        if (PlayerPrefs.HasKey("IsFirstPlay"))
        {
            isFirstPlay = Convert.ToBoolean(PlayerPrefs.GetInt("IsFirstPlay"));

            if (isFirstPlay) continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = false;

            isFirstPlay = true;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenMenu ()
    {
        HideControls();

        isAnimating = true;

        if (GameManager.IsPanelOpen) { return; }

        clips     = animator.runtimeAnimatorController.animationClips;
        delayTime = 0f;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals("open")) delayTime = clip.length;
        }

        menuPanel.SetActive(true);
        animator.SetBool("isOpen", true);

        StopAllCoroutines();
        StartCoroutine(EnableMenu());
        
//        PokiUnitySDK.Instance.gameplayStop();
    }

    public void CloseMenu ()
    {
//        InitiateCommercialBreak(); //POKI
            
        if (assistPanel.activeSelf)
        {
            assistPanel.SetActive(false);
            AssistController.EnemyStrength  = newEnemyStrength;
            AssistController.HazardDamage   = newHazardDamage;
            AssistController.IsInfiniteDash = isInfiniteDashChanged;
            AssistController.IsInvicibility = isInvicibleChanged;

            enemyStrengthText.text = AssistController.EnemyStrength + "%";
            hazardDamageText.text  = AssistController.HazardDamage  + "%";
            staminaDrainText.text  = AssistController.StaminaDrain  + "%";
            infiniteDashText.text  = AssistController.IsInfiniteDash ? "[x]" : "[]";
            invicibleText.text     = AssistController.IsInvicibility ? "[x]" : "[]";

            foreach (Button b in assistButtons) { b.interactable = false; }

            EventSystem.current.SetSelectedGameObject(null);
        }

        if (controlsPanel.activeSelf)
            HideControls();

        canBeClicked   = false;
        Time.timeScale = 1f;
        isAnimating    = true;

        if (canBeClosed == false)
        {
            canBeClosed = true;

            continueButton.interactable = false;
        }

        if (GameManager.IsPanelOpen) { return; }

        clips     = animator.runtimeAnimatorController.animationClips;
        delayTime = 0f;

        foreach (AnimationClip clip in clips)
            if (clip.name.Equals("close"))
                delayTime = clip.length;

        animator.SetBool("isOpen", false);

        StopAllCoroutines();
        StartCoroutine(DisableMenu());
        
//        PokiUnitySDK.Instance.gameplayStart();
    }

    private IEnumerator EnableMenu ()
    {
        if (GameManager.player) GameManager.player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        yield return new WaitForSecondsRealtime(delayTime);
        Time.timeScale = 0f;

        //EnableButtonNavigation();
        //EventSystem.current.SetSelectedGameObject(newGameButton);
        canBeClicked = true;
        isAnimating  = false;
    }

    private IEnumerator DisableMenu ()
    {
        if (GameManager.player) GameManager.player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSecondsRealtime(delayTime);

        menuPanel.SetActive(false);
        isAnimating = false;
        //DisableButtonNavigation();
    }

    public void NewGame () // starts a new game
    {
        if (canBeClicked)
        {
            if (isFirstPlay == false)
                OpenApprovePanel();
            else
            {
                #if UNITY_WEBGL 
                StartGameEvent();
                #endif 
                
                CloseMenu();
                PlayerStats.IsFirstPlay = true;
                ChestsController.ResetChestsState();
                PlayerStats.ResetPlayerState();
                InfoPanel.OpenInfoPanel = true;
                GameManager.Instance.ReloadHouse();

                isFirstPlay = PlayerStats.IsFirstPlay;
            }
        }
    }

    public void ContinueGame () // continues from where you left
    {
        if (canBeClicked)
        {
            StartLevelEvent(1);
            
            PlayerStats.LoadPlayerState();
            ChestsController.LoadChestsState();
            PlayerStats.Instance.UpdateHealth();
            PlayerStats.Instance.UpdateStamina();
            CloseMenu();
            GameManager.player.isDeactivated = false;
        }
    }

    public void OpenApprovePanel ()
    {
        approvePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(disapproveButton.gameObject);
        isApprovePanelOpen = true;
    }

    public void Approve ()
    {
//        InitiateCommercialBreak();  //POKI
        
        approvePanel.SetActive(false);
        PlayerStats.IsFirstPlay = true;
        ChestsController.ResetChestsState();
        PlayerStats.ResetPlayerState();
        InfoPanel.OpenInfoPanel = true;
        PlayerStats.Instance.UpdateHealth();
        PlayerStats.Instance.UpdateStamina();
        isFirstPlay = PlayerStats.IsFirstPlay;
        NewGame();
        EventSystem.current.SetSelectedGameObject(null);
        isApprovePanelOpen = false;
    }

    public void Dissaprove ()
    {
        approvePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        isApprovePanelOpen = false;
    }

    public void CloseApprovePanel ()
    {
        approvePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        isApprovePanelOpen = false;
    }

    public void ShowControls ()
    {
        if (controlsPanel.activeSelf == false) controlsPanel.SetActive(true);

        foreach (Button b in controlButtons) b.interactable = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void HideControls ()
    {
        foreach (Button b in controlButtons) { b.interactable = false; }

        if (controlsPanel.activeSelf) controlsPanel.SetActive(false);
    }

    public void HandleMusic ()
    {
        if (musicDisabledImage.activeSelf == false)
            AudioController.Instance.LowerGlobalMusicVolume();
        else
            AudioController.Instance.RiseGlobalMusicVolume();

        musicDisabledImage.SetActive(!musicDisabledImage.activeSelf);
    }

    public void HandleSounds ()
    {
        if (soundDisabledImage.activeSelf == false)
            AudioController.Instance.LowerGlobalSFXVolume();
        else
            AudioController.Instance.RiselobalSFXGVolume();

        soundDisabledImage.SetActive(!soundDisabledImage.activeSelf);
    }

    public void PlaySelectSFX ()
    {
        if (canBeClicked) AudioController.Instance.ButtonSelectSFX();
    }

    public void PlayHoverSFX ()
    {
        if (canBeClicked) AudioController.Instance.ButtonHoverSFX();
    }

    public void OpenDevblog () { Application.OpenURL("https://bigtinygames.wixsite.com/home"); }

    private void DisableButtonNavigation ()
    {
        foreach (Button b in buttons) b.enabled = false;
    }

    private void EnableButtonNavigation ()
    {
        foreach (Button b in buttons) b.enabled = true;
    }

    #region Assist Buttons

    public void OpenAssistPanel ()
    {
        newEnemyStrength      = AssistController.EnemyStrength;
        newHazardDamage       = AssistController.HazardDamage;
        isInfiniteDashChanged = AssistController.IsInfiniteDash;
        isInvicibleChanged    = AssistController.IsInvicibility;

        assistPanel.SetActive(true);
        HideControls();

        foreach (Button b in assistButtons) { b.interactable = true; }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseAssistPanel ()
    {
        assistPanel.SetActive(false);

        foreach (Button b in assistButtons) { b.interactable = false; }

        EventSystem.current.SetSelectedGameObject(null);

        if (SceneManager.GetActiveScene().buildIndex > 3)
        {
            if (newEnemyStrength         == AssistController.EnemyStrength
                && newHazardDamage       == AssistController.HazardDamage
                && isInfiniteDashChanged == AssistController.IsInfiniteDash
                && isInvicibleChanged    == AssistController.IsInvicibility) { return; }

            HideControls();
            CloseMenu();
        }
    }

    public void ReduceEnemyStrength ()
    {
        AssistController.EnemyStrength -= 10;

        if (AssistController.EnemyStrength < 10) AssistController.EnemyStrength = 100;

        enemyStrengthText.text = AssistController.EnemyStrength + "%";
    }

    public void ReduceHazardDamage ()
    {
        AssistController.HazardDamage -= 10;

        if (AssistController.HazardDamage < 10) AssistController.HazardDamage = 100;

        hazardDamageText.text = AssistController.HazardDamage + "%";
    }

    public void ReduceStaminaDrain ()
    {
        AssistController.StaminaDrain -= 10;

        if (AssistController.StaminaDrain < 10) AssistController.StaminaDrain = 100;

        staminaDrainText.text = AssistController.StaminaDrain + "%";
    }

    public void SetInfiniteDash ()
    {
        AssistController.IsInfiniteDash = !AssistController.IsInfiniteDash;

        infiniteDashText.text = AssistController.IsInfiniteDash ? "[x]" : "[]";
    }

    public void SetInvincibility ()
    {
        AssistController.IsInvicibility = !AssistController.IsInvicibility;

        invicibleText.text = AssistController.IsInvicibility ? "[x]" : "[]";
    }

    public void RestoreDefaults ()
    {
        AssistController.EnemyStrength  = 100;
        AssistController.HazardDamage   = 100;
        AssistController.StaminaDrain   = 100;
        AssistController.IsInfiniteDash = false;
        AssistController.IsInvicibility = false;

        enemyStrengthText.text = AssistController.EnemyStrength + "%";
        hazardDamageText.text  = AssistController.HazardDamage  + "%";
        staminaDrainText.text  = AssistController.StaminaDrain  + "%";
        infiniteDashText.text  = AssistController.IsInfiniteDash ? "[x]" : "[]";
        invicibleText.text     = AssistController.IsInvicibility ? "[x]" : "[]";
    }

    #endregion
    
    //Poki Commercial Break
//    public void commercialBreakComplete() 
//    {
//        AudioListener.volume = 1f;
//    }
//
//    public void InitiateCommercialBreak()
//    {
//        if (PokiUnitySDK.Instance.adsBlocked() == false)
//        {
//            AudioListener.volume = 0f;
//
//            PokiUnitySDK.Instance.commercialBreakCallBack = commercialBreakComplete;
//            PokiUnitySDK.Instance.commercialBreak();
//
//            PlayerStats.IsCommercialBreakInitialized = true;
//        }
//    }

    //Armor Games Links
    public void LinkToArmorGameHomePage()
    {
        //Application.OpenURL("http://armor.ag/MoreGames");
        Application.ExternalEval("window.open('http://armor.ag/MoreGames','_blank')");
    }

    public void LinkToArmorGamesFBPage()
    {
       //Application.OpenURL("http://www.facebook.com/ArmorGames");
       Application.ExternalEval("window.open('https://www.facebook.com/ArmorGames','_blank')");
    }
    
    [DllImport("__Internal") ]
    private static extern void StartGameEvent ();
    
    [DllImport("__Internal") ]
    private static extern void StartLevelEvent (int level);
}