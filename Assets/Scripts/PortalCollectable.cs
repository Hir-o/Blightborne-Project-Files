using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PortalCollectable : MonoBehaviour {

	[SerializeField] private GameObject buttonSprite;
	[SerializeField] private GameObject[] pyramids;

	private enum ItemState { NotCollected, Collected };
	private ItemState itemState = ItemState.NotCollected;
	
	private bool canActivate = false;
	private Player player;
	private BoxCollider2D boxCollider;

	private Animator _animator;

	private void Start()
	{
		_animator = buttonSprite.GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	private void Update() 
	{
		if (canActivate && itemState == ItemState.NotCollected)
		{
			if (InputControl.GetButtonDown("Interact"))
				Collect();
		}	

		if (itemState == ItemState.Collected)
		{
			boxCollider.isTrigger = false;
			boxCollider.enabled = false;
			buttonSprite.SetActive(false);
		}
	}

	public void Collect()
	{
		foreach (GameObject pyramid in pyramids)
		{
			pyramid.SetActive(false);
		}

		GameManager.AllowUseOfPortals = true;		
		itemState = ItemState.Collected;
		AudioController.Instance.CollectSFX();

		GameManager.hud.UpdateBluePortalImage();
		GameManager.hud.UpdateRedPortalImage();
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
		if (player.isDeactivated) { return; }
		
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

        if(other.tag == (Tag.PlayerTag))
        {
            if(player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
        }
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if(other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
			canActivate = false;
			GameManager.Instance.peekDisabled = false;
        }
	}

}
