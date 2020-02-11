using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class LeverSpecial : MonoBehaviour {

	private Sprite leverDown;
	[SerializeField] private Sprite leverUp;
	[SerializeField] private GameObject[] objectsToInteract;
	[SerializeField] private GameObject buttonSprite;

	private bool canActivate = false;
	private Player player;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	private CinemachineCameraShaker cameraShaker;
	private Animator animator;

	[Header("Portal Check")]
	[SerializeField] private float checkForPortalInverval = 1f;
	[SerializeField] private float rangeX = 3.4f;
	[SerializeField] private float rangeY = .5f;
	[SerializeField] private Collider2D[] portals;
	[SerializeField] private LayerMask portalMask;

	[Header("Camera Shake Values")]
	[SerializeField] private float duration = .6f;
	[SerializeField] private float amplitude = .3f;
	[SerializeField] private float frequency = 3f;

	private Animator _animator;

	private void Awake()
	{
		_animator = buttonSprite.GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		cameraShaker = FindObjectOfType<CinemachineCameraShaker>();
	}

	private void Start() 
	{
		leverDown = spriteRenderer.sprite;

		InvokeRepeating(nameof(CheckForPortal), checkForPortalInverval, checkForPortalInverval);
	}

	private void Update()
	{
		if (portals.Length > 0) { return; }

		if (canActivate)
			if (InputControl.GetButtonDown("Interact"))
				InteractWithLever();
	}

	private void CheckForPortal()
	{
		portals = Physics2D.OverlapBoxAll(transform.position, new Vector2(rangeX, rangeY), 0f, portalMask);
	}

	public void InteractWithLever()
	{
		if (spriteRenderer.sprite == leverDown)
			spriteRenderer.sprite = leverUp;
		else if (spriteRenderer.sprite == leverUp)
			spriteRenderer.sprite = leverDown;
		
		foreach (GameObject gate in objectsToInteract)
		{
			animator = gate.GetComponent<Animator>();
			animator.SetBool("isLocked", !animator.GetBool("isLocked"));
		}
		
		GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));
		
		CameraShakeController.Instance.ShakeCameraLever();
		
		AudioController.Instance.GateSFX();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			if (portals.Length == 0)
			{
				player = other.gameObject.GetComponent<Player>();
				buttonSprite.SetActive(true);
				canActivate = true;
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (portals.Length > 0) 
		{
			buttonSprite.SetActive(false);
			canActivate = false;
			return; 
		}

		if (player != null)
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

		if (other.CompareTag(Tag.PlayerTag))
        {
			if (player == null) { return; }
            if (player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if (player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if (player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if (player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
	        
			if (buttonSprite.activeSelf == false)
			{
				player = other.gameObject.GetComponent<Player>();
				buttonSprite.SetActive(true);
				canActivate = true;
			}
        }
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (portals.Length > 0) { return; }

		if (other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
			canActivate = false;
			GameManager.Instance.peekDisabled = false;
        }
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY, 1f));	
	}

}
