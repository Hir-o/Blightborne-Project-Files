using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class LevelExit : MonoBehaviour {

    [SerializeField] float sceneLoadTime = .2f;
    [SerializeField] float objectStillWaitTime = .2f;
    [SerializeField] private GameObject buttonSprite;

    private Player player;
    private Animator animator;
    private bool canActivate = false;

    private enum SceneLoader {Enabled, Disabled};
    private SceneLoader sceneLoader = SceneLoader.Enabled;

    [SerializeField] private bool isFirstDoorToEnter = false;

    [Header("Custom Scene Transition")]
    [SerializeField] private bool isSceneCustom = false;
    [SerializeField] private string sceneName;
    [SerializeField] private bool isPlayerReset = false;

    [Header("Skip Levels Panel (Only for the dungeon entry doors)")]
    [SerializeField] private bool isEntryToTheDungeon, isOpened = false;
    [Range(1,2)][SerializeField] private int dungeonNumber;
    [SerializeField] private GameObject dunegonEntryPanel;
    [SerializeField] private Button yesButton, noButton;

    public enum DoorType {Door, Special}
    public DoorType doorType = DoorType.Door;

    public enum SpecialLevel {None, PuzzleI, PuzzleII, Boss}
    public SpecialLevel specialLevel = SpecialLevel.None;

    [Header("Door Locked Sign")]
    [SerializeField] private GameObject doorLockedSign;

    [Header("Tutorial Level Code")]
    [SerializeField] private bool isTutorialLevel;

    [Header("Key Animator")]
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _animator = buttonSprite.GetComponent<Animator>();
        if (PlayerStats.IsFirstPlay == false)
            isFirstDoorToEnter = false;

        if (isFirstDoorToEnter)
            SetSceneNameToTutorial();

        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();

        if (isEntryToTheDungeon)
        {
            switch (dungeonNumber)
            {
                case 1: noButton.onClick.AddListener(NoButtonClickedFirstDungeon);
                    break;
                case 2: noButton.onClick.AddListener(NoButtonClickedSecondDungeon);
                    break;
                default:
                    break;
            }
        }
    }

    private void Update() 
    {
        if (specialLevel == SpecialLevel.PuzzleI && PlayerStats.HasBlueKey == false)
        {
            if (buttonSprite.activeSelf)
                buttonSprite.SetActive(false);

            return; 
        }
        else if (specialLevel == SpecialLevel.PuzzleI && PlayerStats.HasBlueGem)
        {
            if (buttonSprite.activeSelf)
                buttonSprite.SetActive(false);

            return; 
        }
        else if (specialLevel == SpecialLevel.PuzzleI && animator.GetBool("powerUp") == false)
            animator.SetBool("powerUp", true);

        if (specialLevel == SpecialLevel.PuzzleII && PlayerStats.HasRedKey == false)
        {
            if (buttonSprite.activeSelf)
                buttonSprite.SetActive(false);
 
            return; 
        }
        else if (specialLevel == SpecialLevel.PuzzleII && PlayerStats.HasRedGem)
        {
            if (buttonSprite.activeSelf)
                buttonSprite.SetActive(false);

            return; 
        }
        else if (specialLevel == SpecialLevel.PuzzleII && animator.GetBool("powerUp") == false)
            animator.SetBool("powerUp", true);
            
        if (specialLevel == SpecialLevel.Boss)
        {
            if (PlayerStats.HasBlueGem == false || PlayerStats.HasRedGem == false)
            {
                if (buttonSprite.activeSelf)
                    buttonSprite.SetActive(false);

                return; 
            }
            else if (animator.GetBool("powerUp") == false)
                animator.SetBool("powerUp", true);
        }   

        if (dungeonNumber == 2 && PlayerStats.HasBlueKey == false) 
        {
            if (buttonSprite.activeSelf)
                buttonSprite.SetActive(false);

            return; 
        }
        else if (dungeonNumber == 2)
            doorLockedSign.SetActive(false);

        if (canActivate)
        {
            if (InputControl.GetButtonDown("Interact") && isOpened == false)
            {
                if (GameManager.hud.abilityPanel.activeSelf) { return; }
                
                isOpened = true;
                if (isEntryToTheDungeon)
                {
                    if (dungeonNumber == 1)
                    {
                        if (DungeonRecall.FirstDungeonRecallSceneName != null)
                        {
                            player.isDeactivated = true;
                            dunegonEntryPanel.GetComponent<RestoreFocus>().SelectButton();
                            dunegonEntryPanel.SetActive(true);

                            GameManager.IsDungeonPanelEntryOpen = true;
                            GameManager.Instance.SendLevelExitObj(this);

                            yesButton.onClick.AddListener(() => ButtonClicked(DungeonRecall.FirstDungeonRecallSceneName));
                            return;
                        }
                    }
                    else if (dungeonNumber == 2)
                    {
                        if (DungeonRecall.SecondDungeonRecallSceneName != null)
                        {
                            player.isDeactivated = true;
                            dunegonEntryPanel.GetComponent<RestoreFocus>().SelectButton();
                            dunegonEntryPanel.SetActive(true);

                            GameManager.IsDungeonPanelEntryOpen = true;
                            GameManager.Instance.SendLevelExitObj(this);

                            yesButton.onClick.AddListener(() => ButtonClicked(DungeonRecall.SecondDungeonRecallSceneName));
                            return;
                        }
                    }
                }
                
                if (sceneLoader == SceneLoader.Enabled)
                {
                    sceneLoader = SceneLoader.Disabled;
                    
                    if (isTutorialLevel && PlayerStats.IsFirstPlay)
                        PlayerStats.IsFirstPlay = false;

                    if (GetComponent<Animator>())
                        animator.SetTrigger("open");

                    if (isSceneCustom == false)
                        StartCoroutine("PortalTransition");
                    else
                        StartCoroutine("PortalTransitionCustom");

                    GameManager.cameraTransition.StartSwipeIn();
                }

                if (doorType == DoorType.Door)
                    AudioController.Instance.DoorOpenSFX();
                else if (doorType == DoorType.Special)
                    AudioController.Instance.DoorSpecialOpenSFX();
            }
        }
    }

    private void ButtonClicked(string sceneName)
    {
        GameManager.cameraTransition.StartSwipeIn();

        if (sceneLoader == SceneLoader.Enabled)
        {
            sceneLoader = SceneLoader.Disabled;

            if (GetComponent<Animator>())
                animator.SetTrigger("open");
        }

        GameManager.IsDungeonPanelEntryOpen = false;

        StartCoroutine(PortalTransitionLastLevel(sceneName));
    }

    private void NoButtonClickedFirstDungeon()
    {
        GameManager.cameraTransition.StartSwipeIn();

        DungeonRecall.FirstDungeonRecallSceneName = null;

        if (sceneLoader == SceneLoader.Enabled)
        {
            sceneLoader = SceneLoader.Disabled;

            if (GetComponent<Animator>())
                animator.SetTrigger("open");
        }

        GameManager.IsDungeonPanelEntryOpen = false;

        StartCoroutine("PortalTransitionCustom");
    }

    private void NoButtonClickedSecondDungeon()
    {
        GameManager.cameraTransition.StartSwipeIn();
        
        DungeonRecall.SecondDungeonRecallSceneName = null;

        if (sceneLoader == SceneLoader.Enabled)
        {
            sceneLoader = SceneLoader.Disabled;

            if (GetComponent<Animator>())
                animator.SetTrigger("open");
        }

        GameManager.IsDungeonPanelEntryOpen = false;

        StartCoroutine("PortalTransitionCustom");
    }

    private IEnumerator PortalTransition()
    {
        player.isDeactivated = true;
        
        yield return new WaitForSeconds(objectStillWaitTime);

        player.StopAllAnimations();
        player.TeleportAnimation();

        yield return new WaitForSeconds(sceneLoadTime);

        LevelManager.LoadNextScene();
    }

    private IEnumerator PortalTransitionCustom()
    {
        player.isDeactivated = true;
        
        yield return new WaitForSeconds(objectStillWaitTime);

        player.StopAllAnimations();
        player.TeleportAnimation();

        yield return new WaitForSeconds(sceneLoadTime);

        if (PlayerStats.IsFirstPlay && isFirstDoorToEnter)
            LevelManager.LoadSceneWithName("Tutorial", true);
        else
            LevelManager.LoadSceneWithName(sceneName, isPlayerReset);
    }

    private IEnumerator PortalTransitionLastLevel(string lastSceneName)
    {
        yield return new WaitForSeconds(objectStillWaitTime);

        player.StopAllAnimations();
        player.TeleportAnimation();

        yield return new WaitForSeconds(sceneLoadTime);

        LevelManager.LoadSceneWithName(lastSceneName, isPlayerReset);
    }

    public void CloseDungeonPanel()
    {
        dunegonEntryPanel.SetActive(false);
        GameManager.IsDungeonPanelEntryOpen = false;
        isOpened = false;
        player.isDeactivated = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			player = other.gameObject.GetComponent<Player>();
			buttonSprite.SetActive(true);
			canActivate = true;
		}
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
        if (_animator != null)
        {
            switch (KeyBindings.CurrentProfile)
            {
                case KeyBindings.Profile.Profile1:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
                case KeyBindings.Profile.Profile2:
                    _animator.SetBool("Down", false);
                    _animator.SetBool("E",    true);
                    break;
                case KeyBindings.Profile.Profile3:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
            }
        }
        
        if (other.CompareTag(Tag.PlayerTag))
        {
            if (player.isDeactivated) { return; }

            if(player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping")) { return; }

            GameManager.Instance.peekDisabled = true;
        }
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
			canActivate = false;
            
            GameManager.Instance.peekDisabled = false;
        }
	}

    public void SetSceneNameToTutorial()
    {
        if (PlayerStats.IsFirstPlay)
            sceneName = "Tutorial";
    }

    public void DisableDoor () { this.enabled = false; }
}
