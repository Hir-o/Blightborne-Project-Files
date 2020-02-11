using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SaveCrystal : MonoBehaviour {

	private bool canActivate;
	[SerializeField] private GameObject buttonSprite;

	private float buttonXSize;
	private Player player;

    [Header("Save Disable Interval & Notification")]
    private float savePeriod;
    [SerializeField] private float disableSaveTimer = 3f;
    private bool isSaveDisabled;
    private HUD hud;

    private Animator _animator;

    private void Start()
    {
        player = GameManager.player;
        hud = GameManager.hud;
        _animator = buttonSprite.GetComponent<Animator>();
    }

	private void Update()
    {        
        if (isSaveDisabled)
        {
            if (savePeriod >= disableSaveTimer)
            {
                isSaveDisabled = false;
                savePeriod = 0f;
            }
            else
                savePeriod += Time.deltaTime;
        }

        if(canActivate && InputControl.GetButtonDown("Interact") && isSaveDisabled == false)
		{
            AudioController.Instance.SaveGameSFX();
			ChestsController.SaveChestsState();
			PlayerStats.SavePlayerState();
            isSaveDisabled = true;
            hud.DisplaySaveNotification();
		}
    }

	private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSaveDisabled) 
        {
            canActivate = false;
            buttonSprite.SetActive(false);
            return; 
        }

        if(other.CompareTag(Tag.PlayerTag))
        {
            canActivate = true;
            buttonSprite.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag(Tag.PlayerTag))
        {
            canActivate = false;
            buttonSprite.SetActive(false);
            GameManager.Instance.peekDisabled = false;
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
        
        if (isSaveDisabled) 
        {
            canActivate = false;
            buttonSprite.SetActive(false);
            return; 
        }

        if(other.CompareTag(Tag.PlayerTag))
        {
            canActivate = true;
            buttonSprite.SetActive(true);
        }
        
        buttonXSize = Mathf.Abs(buttonSprite.transform.localScale.x);
        if (this.gameObject.transform.localScale.x > 0f)
        {
            buttonSprite.transform.localScale = new Vector3(buttonXSize, buttonSprite.transform.localScale.y, buttonSprite.transform.localScale.z);
        }
        else if (this.gameObject.transform.localScale.x < 0f)
        {
            buttonSprite.transform.localScale = new Vector3(-buttonXSize, buttonSprite.transform.localScale.y, buttonSprite.transform.localScale.z);
        }

        if (player.isDeactivated) { return; }

        if(other.CompareTag(Tag.PlayerTag))
        {
            if(player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
        }
    }

}
